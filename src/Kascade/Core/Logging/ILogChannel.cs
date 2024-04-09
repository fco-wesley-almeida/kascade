using System.ComponentModel;
using Kascade.Core.Options;

namespace Kascade.Core.Logging;

public interface ILogChannel
{
	public void LogInfo(string content);
	public void LogError(string content);
	public bool IsEnabled();
	public ILogChannel Configure();
}