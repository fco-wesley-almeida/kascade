using System.Net;

namespace Kascade.Core.Options;

public class OptionsCommandLine: IOptions
{
	public IPEndPoint Endpoint { get; private set; } = null!;
	public Uri Destination { get; private set;} = null!;
	
	private OptionsCommandLine() {}

	public static IOptions FromArgs(string[] args)
	{
		var options = new OptionsCommandLine();
		foreach (var (key, value) in args.AsDictionary())
		{
			if (string.Equals(key, nameof(options.Endpoint), StringComparison.OrdinalIgnoreCase))
			{
				var endpointSplited = value.Split(":");
				if (endpointSplited.Length != 2)
				{
					throw new ArgumentException("Endpoint provided is invalid. Should be something like: 0.0.0.0:8080");
				}

				if (!IPAddress.TryParse(endpointSplited[0], out var ip))
				{
					throw new ArgumentException("Invalid ip provided");
				}
				
				if (!int.TryParse(endpointSplited[1], out int port))
				{
					throw new ArgumentException("Invalid port provided");
				}
				options.Endpoint = new(ip, port);
			}

			if (string.Equals(key, nameof(options.Destination), StringComparison.OrdinalIgnoreCase))
			{
				if (!Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
				{
					throw new ArgumentException("Invalid destination provided");
				}
				options.Destination = uri;
			}
		}

		if (options.Endpoint is null)
		{
			throw new ArgumentException("Endpoint is required");
		}
		
		if (options.Destination is null)
		{
			throw new ArgumentException("Destination is required");
		}
		return options;
	}
}