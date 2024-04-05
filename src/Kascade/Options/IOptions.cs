using System.Net;

namespace Kascade.Options;

public interface IOptions
{
	public IPEndPoint Endpoint { get; }
	public Uri Destination { get; }
}