using System.Net;

namespace Hermes.Options;

public interface IOptions
{
	public IPEndPoint Endpoint { get; }
	public Uri Destination { get; }
}