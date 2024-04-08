namespace Kascade.Core.Options;

public static class StringExtensions
{
	public static Dictionary<string, string> AsDictionary(this string[] args)
	{
		var dictionary = new Dictionary<string, string>();
		for (var i = 0; i < args.Length; i++)
		{
			if (args[i].Contains("--") && i < args.Length - 1 && !args[i + 1].Contains("--"))
			{
				dictionary.Add(
					key: args[i].Replace("--", ""), 
					value: args[i + 1]
				);
			}
		}
		return dictionary;
	}
}