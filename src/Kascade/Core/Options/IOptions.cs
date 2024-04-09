using System.Net;

namespace Kascade.Core.Options;

public enum LogChannel
{
	Void, 
	Console
}
public interface IOptions
{
	public IPEndPoint Endpoint { get; }
	public Uri Destination { get; }
	public LogChannel LogChannel { get; }
}