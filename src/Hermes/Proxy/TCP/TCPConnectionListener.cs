using System.Net;
using System.Net.Sockets;

namespace Hermes.Proxy.TCP;

/// <summary>
/// Listens for incoming TCP connections on a specified endpoint and delegates handling to a provided ITcpConnectionHandler.
/// </summary>
public class TcpConnectionListener
{
	private readonly IPEndPoint _endpoint;
	private readonly ITcpConnectionHandler _handler;
	private const int Backlog = 10;

	/// <summary>
	/// Initializes a new instance of the TcpConnectionListener class with the specified endpoint and connection handler.
	/// </summary>
	/// <param name="endpoint">The endpoint to listen on for incoming connections.</param>
	/// <param name="handler">The handler responsible for processing incoming connections.</param>
	public TcpConnectionListener(IPEndPoint endpoint, ITcpConnectionHandler handler)
	{
		_handler = handler;
		_endpoint = endpoint;
	}

	/// <summary>
	/// Starts listening for incoming TCP connections.
	/// </summary>
	public void Listen()
	{
		var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Blocking = false;
		socket.Bind(_endpoint);
		socket.Listen(Backlog);
		socket.BeginAccept(_handler.AcceptCallback, socket);
	}
}