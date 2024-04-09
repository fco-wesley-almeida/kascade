using Kascade.Core.Options;

namespace Kascade.Core.Logging;

public class VoidLogChannel: ILogChannel
{
	private readonly IOptions _options;

	public VoidLogChannel(IOptions options)
	{
		_options = options;
	}

	public void LogInfo(string content)
	{
		// Do nothing
	}

	public void LogError(string content)
	{
		// Do nothing
	}

	public bool IsEnabled() => _options.LogChannel == LogChannel.Void;

	public ILogChannel Configure()
	{
		Console.WriteLine($"Configuring {nameof(VoidLogChannel)}...");
		// Do nothing
		return this;
	}
}