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
        public Func<TContext, TInType, Task<NetStreamResult<TOutType>>> WrappedFunctor { get; }
        public Func<TContext, NetStreamResult<TOutType>, Task<NetStreamResult<TOutType>>> PostProcessor { get; }

        public WrappedPipelineStep(
            NetStreamConfiguration config,
            Func<TContext, TInType, Task<NetStreamResult<TInType>>> preProcessor,
            Func<TContext, TInType, Task<NetStreamResult<TOutType>>> functor,
            Func<TContext, NetStreamResult<TOutType>, Task<NetStreamResult<TOutType>>> postProcessor) : base(config, functor)
        {
            PreProcessor = preProcessor;
            WrappedFunctor = functor;
            PostProcessor = postProcessor;
        }

        private async Task<NetStreamResult<TOutType>> WrapFunctor(TContext context, TInType inbound)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            NetStreamResult<TInType> preResult;
            try
            {
                preResult = await PreProcessor(context, inbound);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exc)
            {
                preResult = new NetStreamResult<TInType>(exc);
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            return preResult.Match(
                preValue =>
                {
                    NetStreamResult<TOutType> functorResult;
                    try
                    {
                        functorResult = WrappedFunctor(context, preValue).Result;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exc)
                    {
                        functorResult = new NetStreamResult<TOutType>(exc);
                    }

                    context.CancellationToken.ThrowIfCancellationRequested();

                    return PostProcessor(context, functorResult).Result;
                },
                exc =>
                {
                    try
                    {
                        return PostProcessor(context, new NetStreamResult<TOutType>(exc)).Result;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exc2)
                    {
                        return new NetStreamResult<TOutType>(exc2);
                    }
                },
                cancellation =>
                {
                    try
                    {
                        return PostProcessor(context, new NetStreamResult<TOutType>(cancellation)).Result;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exc)
                    {
                        return new NetStreamResult<TOutType>(exc);
                    }
                });
        }
    }
}
