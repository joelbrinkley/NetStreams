using System;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
    internal class HandleFunction<T, TResponse> : IHandle<T, TResponse>
        where T : IMessage
        where TResponse : IMessage
    {
        IStreamWriter<TResponse> _writer;
        Func<IConsumeContext<T>, TResponse> _handle;
        INetStream<T> _stream;

        public INetStream<T> Stream => _stream;

        public HandleFunction(Func<IConsumeContext<T>, TResponse> handle, INetStream<T> netStream)
        {
            _handle = handle;
            _stream = netStream;
        }

        public async Task Handle(IConsumeContext<T> consumeContext)
        {
            var response = _handle(consumeContext);
            await _writer.WriteAsync(response);
        }

        public void Write(IStreamWriter<TResponse> writer)
        {
            _writer = writer;
        }
    }

    internal class HandleAction<T> : IHandle<T> where T : IMessage
    {
        private Action<IConsumeContext<T>> _handle;
        private INetStream<T> _stream;

        public INetStream<T> Stream => _stream;

        public HandleAction(Action<IConsumeContext<T>> handle, INetStream<T> stream)
        {
            _handle = handle;
            _stream = stream;
        }

        public Task Handle(IConsumeContext<T> consumeContext)
        {
            _handle(consumeContext);
            return Task.CompletedTask;
        }

    }
}