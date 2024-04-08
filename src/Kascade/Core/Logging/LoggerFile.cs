using System.Runtime.InteropServices.Marshalling;
using Kascade.Core.Options;

namespace Kascade.Core.Logging;

public class LoggerFile: ILogger
{

	private readonly IOptions _options;
	public LoggerFile(IOptions options)
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

	public bool IsEnabled() => true;

	public ILogger Configure()
	{
		Console.WriteLine($"Configuring {nameof(LoggerFile)}...");
		return this;
	}
}