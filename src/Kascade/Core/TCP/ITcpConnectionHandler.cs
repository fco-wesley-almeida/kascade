using System.Net.Sockets;

namespace Kascade.Core.TCP;

public interface ITcpConnectionHandler
{
	public bool TryHandle(Socket client, out byte[]? bytesResponse);
	public TcpProtocol TcpProtocol {get;}
}