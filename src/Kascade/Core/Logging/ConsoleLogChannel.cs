using System.Runtime.InteropServices.Marshalling;
using Kascade.Core.Options;

namespace Kascade.Core.Logging;

public class ConsoleLogChannel: ILogChannel
{
	private readonly IOptions _options;
	public ConsoleLogChannel(IOptions options)
	{
		_options = options;
	}

	public void LogInfo(string content)
	{
		Console.WriteLine($" {DateTime.Now:s} INFO: {content}");
	}

	public void LogError(string content)
	{
		Console.WriteLine($" {DateTime.Now:s} ERROR: {content}");
	}

	public bool IsEnabled() => _options.LogChannel == LogChannel.Console;

	public ILogChannel Configure()
	{
		Console.WriteLine($"Configuring {nameof(ConsoleLogChannel)}...");
		return this;
	}
}