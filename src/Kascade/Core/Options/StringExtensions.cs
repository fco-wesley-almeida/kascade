namespace Kascade.Core.Options;

public static class StringExtensions
{
	public static Dictionary<string, string> AsDictionary(this string[] args)
	{
		var dictionary = new Dictionary<string, string>();
		foreach (var arg in args.Where(arg => arg.Contains("--")))
		{
			var argSplited = arg.Split("=");
			if (argSplited.Length >= 2)
			{
				dictionary.Add(
					key: argSplited[0].Replace("--", ""),
					value: argSplited[1]
				);
			}
		}
		return dictionary;
	}
}