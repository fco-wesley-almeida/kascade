namespace Kascade.Core.Logging;

public interface ILogger
{
	public void LogInfo(string content);
	public void LogError(string content);
	public bool IsEnabled();
	public ILogger Configure();
}