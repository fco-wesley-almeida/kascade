using System.Net;
using System.Net.Sockets;
using Kascade.Core.Logging;
using Kascade.Features.Proxy.Http;

namespace Kascade.Core.TCP;

/// <summary>
/// Listens for incoming TCP connections on a specified endpoint and delegates handling to a provided ITcpConnectionHandler.
/// </summary>
public class TcpConnectionListener
{
	private readonly IPEndPoint _endpoint;
	private readonly ITcpConnectionHandler _handler;
	private readonly ILogChannel _logChannel;
	private const int Backlog = 1000;
	
	private readonly Queue<IAsyncResult> _requestsQueue = new();
	private readonly object _threadFlag = new();
	private Thread? _mainThread;


	/// <summary>
	/// Initializes a new instance of the TcpConnectionListener class with the specified endpoint and connection handler.
	/// </summary>
	/// <param name="endpoint">The endpoint to listen on for incoming connections.</param>
	/// <param name="handler">The handler responsible for processing incoming connections.</param>
	/// <param name="logChannel"></param>
	public TcpConnectionListener(IPEndPoint endpoint, ITcpConnectionHandler handler, ILogChannel logChannel)
	{
		_handler = handler;
		_logChannel = logChannel;
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
		socket.BeginAccept(AcceptCallback, socket);
	}
	
	/// <summary>
	/// Callback method invoked when a new TCP connection is accepted.
	/// </summary>
	/// <param name="asyncResult">The result of the asynchronous operation.</param>
	private void AcceptCallback(IAsyncResult asyncResult)
	{
		if (_mainThread is null)
		{
			_mainThread = new(HandleQueueMessages);
			_mainThread.Start();
		}

		lock (_requestsQueue)
		{
			_requestsQueue.Enqueue(asyncResult);
		}
	}

	private void HandleQueueMessages()
	{
		int countRequests = 0;
		do
		{
			IAsyncResult? request;

			bool hasPendingRequests;
			lock (_requestsQueue)
			{
				hasPendingRequests = _requestsQueue.TryDequeue(out request);
			}

			if (hasPendingRequests)
			{
				_logChannel.LogInfo($"\n\n[HttpCallHandler] {++countRequests} Dequeue worked!");
				var listener = (Socket?)request!.AsyncState!;
				listener.BeginAccept(AcceptCallback, listener);
				var client = listener.EndAccept(request);
				_logChannel.LogInfo($"Client connected: {client.RemoteEndPoint!}");
				if (_handler.TryHandle(client, out var destResponse))
				{
					client.BeginSend(destResponse!, 0, destResponse!.Length, 0, SendCallback, client);
				}
				else
				{
					client.BeginSend(Array.Empty<byte>(), 0, 0, 0, SendCallback, client);
				}
			}
		} while (true);
	}

	/// <summary>
	/// Callback method invoked when sending data to the client is completed.
	/// </summary>
	/// <param name="asyncResult">The result of the asynchronous operation.</param>
	private void SendCallback(IAsyncResult asyncResult)
	{
		var handler = (Socket) asyncResult.AsyncState!;
		try
		{
			var now = new DateTime();
			int bytesSent = handler.EndSend(asyncResult);
			_logChannel.LogInfo($"Sent {bytesSent} bytes to client after {(new DateTime() - now).Milliseconds}ms.");
			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		}
		catch (Exception e)
		{
			_logChannel.LogError(e.Message);
		}
		finally
		{
			_logChannel.LogInfo($"Connection closed.");
		}
	}
}