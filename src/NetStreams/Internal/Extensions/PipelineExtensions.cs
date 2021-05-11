using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NetStreams.Internal.Pipeline;

namespace NetStreams.Internal.Extensions
{
	public static class PipelineStepExtensions
	{
		public static PipelineStep<TContext, TInType, TOutType> Filter<TContext, TInType, TOutType>(
            this PipelineStep<TContext, TInType, TOutType> pipeline, Func<TContext, TOutType, bool> predicate)
			where TContext : ICancellationTokenCarrier
        {
            return pipeline.Then((context, inbound) =>
            {
                NetStreamResult<TOutType> result;
                try
				{
                    result = predicate(context, inbound)
                        ? new NetStreamResult<TOutType>(inbound)
                        : new NetStreamResult<TOutType>(new StreamCancellation {Reason = "Filter returned False"});
                }
                catch (Exception exc)
                {
                    result = new NetStreamResult<TOutType>(exc);
                }

                return Task.FromResult(result);
            });
        }

		public static PipelineStep<TContext, TInType, TOutType> Handle<TContext, TInType, TOutType>(
            this PipelineStep<TContext, TInType, TOutType> pipeline, Func<TContext, TOutType, Task> processor)
			where TContext : ICancellationTokenCarrier
		{
			return pipeline.Then(async (context, inbound) =>
			{
				try
				{
					await processor(context, inbound);
					return new NetStreamResult<TOutType>(inbound);
				}
				catch (Exception exc)
				{
					return new NetStreamResult<TOutType>(exc);
				}
			});
		}

		public static PipelineStep<TContext, TInType, TNextType> Transform<TContext, TInType, TOutType, TNextType>(
            this PipelineStep<TContext, TInType, TOutType> pipeline, Func<TContext, TOutType, Task<TNextType>> transformer)
		    where TContext : ICancellationTokenCarrier
		{
			return pipeline.Then(async (context, inbound) =>
			{
				TNextType result;
				try
				{
					result = await transformer(context, inbound);
					return new NetStreamResult<TNextType>(result);
				}
				catch (Exception exc)
				{
					return new NetStreamResult<TNextType>(exc);
				}
			});
		}
    }
}
