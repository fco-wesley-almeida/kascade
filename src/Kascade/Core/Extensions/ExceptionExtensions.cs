namespace Kascade.Core.Extensions;

public static class ExceptionExtensions
{
	public static string Summary(this Exception exception)
	{
		return $"[Type={exception.GetType().FullName};Message={exception.Message};StackTrace={exception.StackTrace?.Trim()}]";
	}
}