using System.Net.Sockets;
using System.Text;
using WProxy.TCP;

namespace WProxy.Http;

public class HttpCallHandler: ITcpConnectionHandler
{
	private readonly Uri _uri;
	private readonly List<Thread> _threads = new();

	public HttpCallHandler(Uri uri)
	{
		_uri = uri;
	}

	public TcpProtocol TcpProtocol => TcpProtocol.Http;
	
	public void AcceptCallback(IAsyncResult asyncResult)
	{
		var listener = (Socket)asyncResult.AsyncState!;
		listener.BeginAccept(AcceptCallback, listener);
		
		var thread = new Thread(HandleTcpRequest);
		thread.Start();

		lock (_threads)
		{
			_threads.Add(thread);
		}

		void HandleTcpRequest()
		{
			var client = listener.EndAccept(asyncResult);
			Console.WriteLine($"\nClient connected: {client.RemoteEndPoint!}");
			
			// Get HTTP request headers
			byte[] httpRequestHeaders = client.HttpRequestHeaders();
			
			// Send HTTP request
			Socket destSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			byte[] destResponse = destSocket.SendHttpRequest(httpRequestHeaders, _uri);
			
			// Send response to client
			client.BeginSend(destResponse, 0, destResponse.Length, 0, SendCallback, client);
		}
	}

	private void SendCallback(IAsyncResult asyncResult)
	{
		var handler = (Socket) asyncResult.AsyncState!;
		try
		{
			int bytesSent = handler.EndSend(asyncResult);
			Console.WriteLine($"Sent {bytesSent} bytes to client.");
			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
		finally
		{
			Console.WriteLine($"Connection closed.");
		}
	}
}