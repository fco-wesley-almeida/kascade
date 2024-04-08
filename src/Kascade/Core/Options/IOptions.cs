using System.Net;

namespace Kascade.Core.Options;

public interface IOptions
{
	public IPEndPoint Endpoint { get; }
	public Uri Destination { get; }
}