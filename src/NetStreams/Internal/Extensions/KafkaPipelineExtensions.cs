using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetStreams.Internal.Extensions
{
    public static class KafkaPipelineExtensions
    {
        public static PipelineStep<TContext, TInType, TOutType> ToTopic<TContext, TInType, TOutKeyType, TOutType>(
            this PipelineStep<TContext, TInType, TOutType> pipeline,
            string topic,
            Func<TOutType, TOutKeyType> resolveKey
         ) where TContext : ICancellationTokenCarrier
        {
            var producer = new ProducerFactory().Create<TOutKeyType, TOutType>(topic, pipeline.Configuration);

            var writer = new KafkaTopicWriter<TOutKeyType, TOutType>(producer, resolveKey);

            return pipeline.Then(async (context, inbound) =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                try
                {
                    await writer.WriteAsync(inbound);
                    return new NetStreamResult<TOutType>(inbound);
                }
                catch (Exception exc)
                {
                    return new NetStreamResult<TOutType>(exc);
                }
            });
        }

        public static Task StartAsync<TInKeyType, TInType, TOutType>(
            this PipelineStep<ConsumeContext, TInType, TOutType> pipeline,
            CancellationToken token
        )
        {  
            var consumer = new ConsumerFactory().Create<TInKeyType, TInType>(pipeline.Configuration);

            var topicCreator = new TopicCreator(pipeline.Configuration);

            if (pipeline.Configuration.TopicCreationEnabled)
            {
                topicCreator.CreateAll(pipeline.Configuration.TopicConfigurations).Wait(token);
            }

            var topic = pipeline.Configuration.TopicConfigurations.First().Name;
            consumer.Subscribe(topic);

            return Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(100);
                        Func<long> getLag = () => consumer.GetWatermarkOffsets(consumeResult.TopicPartition).High.Value - consumeResult.TopicPartitionOffset.Offset.Value - 1;

                        var consumeContext = new ConsumeContext(
                            token,
                            pipeline.Configuration.ConsumerGroup,
                            topic,
                            consumeResult.Partition,
                            consumeResult.Offset,
                            getLag);
                         
                        if (consumeResult != null)
                        {
                            await pipeline.Functor(consumeContext, consumeResult.Message.Value).ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        _onError(ex);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }
    }
}
