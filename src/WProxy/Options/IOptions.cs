using System.Net;

namespace WProxy.Options;

public interface IOptions
{
	public IPEndPoint Endpoint { get; }
	public Uri Destination { get; }
}