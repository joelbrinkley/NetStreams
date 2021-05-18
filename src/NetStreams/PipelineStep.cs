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

        public static async Task<NetStreamResult<TOutType>> RunFunctor<TContext, TInType, TOutType>(TContext context, TInType inbound, Func<TContext, TInType, Task<NetStreamResult<TOutType>>> functor)
            where TContext : ICancellationTokenCarrier
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await functor(context, inbound);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exc)
            {
                return new NetStreamResult<TOutType>(exc);
            }
        }

        public PipelineStep<TContext, TInType, TNextType> Then<TNextType>(Func<TContext, TOutType, Task<NetStreamResult<TNextType>>> nextFunctor)
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
            return new PipelineStep<TContext, TInType, TNextType>(Configuration, combined);
        }

        public virtual async Task ExecuteAsync(TContext context, TInType inbound)
        {
            await Functor(context, inbound);
        }
    }
}
