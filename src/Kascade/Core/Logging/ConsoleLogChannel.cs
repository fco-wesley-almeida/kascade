using Kascade.Core.Options;

namespace Kascade.Core.Logging;

public class ConsoleLogChannel: ILogChannel
{
	private readonly IOptions _options;
	private readonly Queue<(string Type, string Content)> _messages;
	private Thread? _thread = null!;
	public ConsoleLogChannel(IOptions options)
	{
		_options = options;
		_messages = new();
	}

	public void LogInfo(string content)
	{
		if (_thread is null)
		{
			_thread = new(LoopConsole);
			_thread.Start();
		}

		lock (_messages)
		{
			_messages.Enqueue(("INFO", content));
		}
	}

	private void LoopConsole()
	{
		Console.WriteLine("THREAD STARTED");
		while (true)
		{
			bool dequeueWorked;
			(string Type, string Content) msg;
			lock (_messages)
			{
				dequeueWorked = _messages.TryDequeue(out msg); 
			}
			if (dequeueWorked)
			{
				Console.WriteLine($" {DateTime.Now:s} {msg.Type}: {msg.Content}");
			}
		}
		// ReSharper disable once FunctionNeverReturns
	}

	public void LogError(string content)
	{
		if (_thread is null)
		{
			_thread = new(LoopConsole);
			_thread.Start();
		}

		lock (_messages)
		{
			_messages.Enqueue(("ERROR", content));
		}
	}

	public bool IsEnabled() => _options.LogChannel == LogChannel.Console;

	public ILogChannel Configure()
	{
		Console.WriteLine($"Configuring {nameof(ConsoleLogChannel)}...");
		return this;
	}
}