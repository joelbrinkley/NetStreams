using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Configuration;

namespace NetStreams
{
    public class WrappedPipelineStep<TContext, TInType, TOutType> : PipelineStep<TContext, TInType, TOutType> where TContext : ICancellationTokenCarrier
    {
        public Func<TContext, TInType, Task<NetStreamResult<TInType>>> PreProcessor { get; }
        public Func<TContext, TInType, Task<NetStreamResult<TOutType>>> UnwrappedFunctor { get; }
        public Func<TContext, NetStreamResult<TOutType>, Task<NetStreamResult<TOutType>>> PostProcessor { get; }

        public WrappedPipelineStep(
            NetStreamConfiguration config,
            Func<TContext, TInType, Task<NetStreamResult<TInType>>> preProcessor,
            Func<TContext, TInType, Task<NetStreamResult<TOutType>>> functor,
            Func<TContext, NetStreamResult<TOutType>, Task<NetStreamResult<TOutType>>> postProcessor) : base(config, functor)
        {
            PreProcessor = preProcessor;
            UnwrappedFunctor = functor;
            PostProcessor = postProcessor;
        }

        public override WrappedPipelineStep<TContext, TInType, TNextType> Then<TNextType>(Func<TContext, TOutType, Task<NetStreamResult<TNextType>>> nextFunctor)
        {
            Func<TContext, TInType, Task<NetStreamResult<TNextType>>> combined = async (context, inbound) =>
            {
                var intermediateResult = await RunFunctor(context, inbound, Functor);

                return intermediateResult.Match<NetStreamResult<TNextType>>(
                    result => RunFunctor(context, result, nextFunctor).Result,
                    exc => new NetStreamResult<TNextType>(exc),
                    cancellation => new NetStreamResult<TNextType>(cancellation)
                );
            };
            return new WrappedPipelineStep<TContext, TInType, TNextType>(Configuration, combined);
        }

        public override async Task ExecuteAsync(TContext context, TInType inbound)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            NetStreamResult<TInType> preResult = await RunFunctor(context, inbound, PreProcessor);

            context.CancellationToken.ThrowIfCancellationRequested();

            preResult.Match(
                preValue =>
                {
                    NetStreamResult<TOutType> functorResult = RunFunctor(context, preValue, UnwrappedFunctor).Result;
                    context.CancellationToken.ThrowIfCancellationRequested();
                    return PostProcessor(context, functorResult).Result;
                },
                exc => RunFunctor(context, new NetStreamResult<TOutType>(exc), PostProcessor).Result,
                cancellation => RunFunctor(context, new NetStreamResult<TOutType>(cancellation), PostProcessor).Result);
        }
    }
}
