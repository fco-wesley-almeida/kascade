using System.Net.Sockets;
using Kascade.Core.Logging;
using Kascade.Core.TCP;

namespace Kascade.Features.Proxy.Http;

/// <summary>
/// Handles incoming HTTP calls by processing TCP connections.
/// </summary>
public class HttpCallHandler: ITcpConnectionHandler
{
	private readonly Uri _uri;
	private readonly ILogChannel _logChannel;

	/// <summary>
	/// Initializes a new instance of the HttpCallHandler class with the specified URI.
	/// </summary>
	/// <param name="uri">The URI of the target server.</param>
	/// <param name="logChannel"></param>
	public HttpCallHandler(Uri uri, ILogChannel logChannel)
	{
		_uri = uri;
		_logChannel = logChannel;
	}

	/// <summary>
	/// Gets the TCP protocol associated with HTTP.
	/// </summary>
	public TcpProtocol TcpProtocol => TcpProtocol.Http;
	
	/// <summary>
	/// Callback method invoked when a new TCP connection is accepted.
	/// </summary>
	/// <param name="asyncResult">The result of the asynchronous operation.</param>
	public void AcceptCallback(IAsyncResult asyncResult)
	{
		var listener = (Socket)asyncResult.AsyncState!;
		listener.BeginAccept(AcceptCallback, listener);
		
		var thread = new Thread(HandleTcpRequest);
		thread.Start();

		// lock (_threads)
		// {
		// 	_threads.Add(thread);
		// }

		void HandleTcpRequest()
		{
			var client = listener.EndAccept(asyncResult);
			_logChannel.LogInfo($"\nClient connected: {client.RemoteEndPoint!}");
			
			// Get HTTP request headers
			byte[] httpRequestHeaders = client.HttpRequestHeaders();
			
			// Send HTTP request
			Socket destSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			byte[] destResponse = destSocket.SendHttpRequest(httpRequestHeaders, _uri, _logChannel);
			
			// Send response to client
			client.BeginSend(destResponse, 0, destResponse.Length, 0, SendCallback, client);
		}
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