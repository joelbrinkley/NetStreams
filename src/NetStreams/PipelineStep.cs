using System;
using System.Threading.Tasks;
using NetStreams.Configuration;
using OneOf;

namespace NetStreams
{
    public class PipelineStep<TContext, TInType, TOutType> where TContext : ICancellationTokenCarrier
    {
        public NetStreamConfiguration Configuration { get; }
        public Func<TContext, TInType, Task<NetStreamResult<TOutType>>> Functor { get; }

        public PipelineStep(NetStreamConfiguration config, Func<TContext, TInType, Task<NetStreamResult<TOutType>>> functor)
        {
            Configuration = config;
            Functor = functor;
        }

        public virtual PipelineStep<TContext, TInType, TNextType> Then<TNextType>(Func<TContext, TOutType, Task<NetStreamResult<TNextType>>> nextFunctor)
        {
            Func<TContext, TInType, Task<NetStreamResult<TNextType>>> combined = async (context, inbound) =>
            {
                NetStreamResult<TOutType> intermediateResult;
                
                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    intermediateResult = await Functor(context, inbound);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exc)
                {
                    return new NetStreamResult<TNextType>(exc);
                }

                context.CancellationToken.ThrowIfCancellationRequested();

                return intermediateResult.Match<NetStreamResult<TNextType>>(
                    result =>
                    {
                        try
                        {
                            return nextFunctor(context, result).Result;
                        }
                        catch (OperationCanceledException)
                        {
                            throw;
                        }
                        catch (Exception exc2)
                        {
                            return new NetStreamResult<TNextType>(exc2);
                        }
                    },
                    exc => new NetStreamResult<TNextType>(exc),
                    cancellation => new NetStreamResult<TNextType>(cancellation)
                );
            };
            return new PipelineStep<TContext, TInType, TNextType>(Configuration, combined);
        }

        public PipelineStep<TContext, TInType, TNextType> AfterProcessing<TNextType>(Func<TContext, NetStreamResult<TOutType>, Task<NetStreamResult<TNextType>>> preFunctor)
        {
            Func<TContext, TInType, Task<NetStreamResult<TNextType>>> combined = async (context, inbound) =>
            {
                NetStreamResult<TOutType> intermediateResult;

                context.CancellationToken.ThrowIfCancellationRequested();

                try
                {
                    intermediateResult = await Functor(context, inbound);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exc)
                {
                    intermediateResult = new NetStreamResult<TOutType>(exc);
                }

                context.CancellationToken.ThrowIfCancellationRequested();

                return await preFunctor(context, intermediateResult);
            };
            return new PipelineStep<TContext, TInType, TNextType>(Configuration, combined);
        }

        public Func<TContext, TInType, Task> Build()
        {
            return async (context, inbound) => await Functor(context, inbound);
        }
    }
}
