using System;
using System.Threading.Tasks;

namespace NetStreams.Internal
{
	internal class HandleFunctionTask<TKey, TMessage, TResponseKey, TResponse> : IHandle<TKey, TMessage, TResponseKey, TResponse>
	{
		IStreamWriter<TKey, TMessage, TResponseKey, TResponse> _writer;
		Func<IConsumeContext<TKey, TMessage>, Task<TResponse>> _handle;
		INetStream<TKey, TMessage> _stream;

		public INetStream<TKey, TMessage> Stream => _stream;

		public HandleFunctionTask(Func<IConsumeContext<TKey, TMessage>, Task<TResponse>> handle, INetStream<TKey, TMessage> netStream)
		{
			_handle = handle;
			_stream = netStream;
		}

		public async Task Handle(IConsumeContext<TKey, TMessage> consumeContext)
		{
			var response = await _handle(consumeContext);
			await _writer.WriteAsync(response);
		}

		public void Write(IStreamWriter<TKey, TMessage, TResponseKey, TResponse> writer)
		{
			_writer = writer;
		}
	}

	internal class HandleFunction<TKey, TMessage, TResponseKey, TResponse> : IHandle<TKey, TMessage, TResponseKey, TResponse>
	{
		IStreamWriter<TKey, TMessage, TResponseKey, TResponse> _writer;
		Func<IConsumeContext<TKey, TMessage>, TResponse> _handle;
		INetStream<TKey, TMessage> _stream;

		public INetStream<TKey, TMessage> Stream => _stream;

		public HandleFunction(Func<IConsumeContext<TKey, TMessage>, TResponse> handle, INetStream<TKey, TMessage> netStream)
		{
			_handle = handle;
			_stream = netStream;
		}

		public async Task Handle(IConsumeContext<TKey, TMessage> consumeContext)
		{
			var response = _handle(consumeContext);
			await _writer.WriteAsync(response);
		}

		public void Write(IStreamWriter<TKey, TMessage, TResponseKey, TResponse> writer)
		{
			_writer = writer;
		}
	}

	internal class HandleAction<TKey, TMessage> : IHandle<TKey, TMessage>
	{
		private Action<IConsumeContext<TKey, TMessage>> _handle;
		private INetStream<TKey, TMessage> _stream;

		public INetStream<TKey, TMessage> Stream => _stream;

		public HandleAction(Action<IConsumeContext<TKey, TMessage>> handle, INetStream<TKey, TMessage> stream)
		{
			_handle = handle;
			_stream = stream;
		}

		public Task Handle(IConsumeContext<TKey, TMessage> consumeContext)
		{
			_handle(consumeContext);
			return Task.CompletedTask;
		}


	}
	internal class HandleActionTask<TKey, TMessage> : IHandle<TKey, TMessage>
	{
		private Func<IConsumeContext<TKey, TMessage>, Task> _handle;
		private INetStream<TKey, TMessage> _stream;

		public INetStream<TKey, TMessage> Stream => _stream;

		public HandleActionTask(Func<IConsumeContext<TKey, TMessage>, Task> handle, INetStream<TKey, TMessage> stream)
		{
			_handle = handle;
			_stream = stream;
		}

		public async Task Handle(IConsumeContext<TKey, TMessage> consumeContext)
		{
			await _handle(consumeContext);
		}

	}
}