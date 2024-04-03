using System.Net;
using System.Net.Sockets;

namespace WProxy.TCP;

public class TcpConnectionListener
{
	private readonly IPEndPoint _endpoint;
	private readonly ITcpConnectionHandler _handler;
	private const int Backlog = 10;

	public TcpConnectionListener(IPEndPoint endpoint, ITcpConnectionHandler handler)
	{
		_handler = handler;
		_endpoint = endpoint;
	}

	public void Listen()
	{
		var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Blocking = false;
		socket.Bind(_endpoint);
		socket.Listen(Backlog);
		socket.BeginAccept(_handler.AcceptCallback, socket);
	}
}