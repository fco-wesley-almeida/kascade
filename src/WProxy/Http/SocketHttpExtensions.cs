using System.Net.Sockets;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace WProxy.Http;

public static class SocketHttpExtensions
{
	
	public static byte[] HttpRequestHeaders(this Socket socket)
	{
		Queue<byte> requestBytes = new();
		var transferenceStarted = false;
		while (true)
		{
			var buffer = new byte[socket.Available];
			int bytesRead = socket.Available > 0 ? socket.Receive(buffer) : 0;
			Console.WriteLine($"Client bytesRead = {bytesRead}");
			if (bytesRead > 0 && !transferenceStarted)
			{
				transferenceStarted = true;
			}
			if (bytesRead == 0 && transferenceStarted)
			{
				break;
			}
			foreach (var @byte in buffer)
			{
				requestBytes.Enqueue(@byte);
			}
		}
		return requestBytes.ToArray();
	}
	
	public static byte[] SendHttpRequest(this Socket socket, byte[] requestBytes, Uri uri)
	{
		Queue<byte> responseBytes = new();
		socket.Connect(uri.Host, uri.Port);
		socket.Send(requestBytes);
		
		byte lastByte = (byte)'\n';
		bool headerWasLoaded = false;
		int contentLength = -1;
		while(contentLength == -1 ? socket.Available > 0 || lastByte == '\n' : responseBytes.Count < contentLength)
		{
			byte[] buffer = new byte[socket.Available];
			int serverBytesRead = socket.Available > 0 ? socket.Receive(buffer) : 0;
			foreach (var @byte in buffer)
			{
				responseBytes.Enqueue(@byte);
			}
			
			// Console.WriteLine(serverResponseStr);
			if (serverBytesRead > 0)
			{
				lastByte = buffer[serverBytesRead - 1];
			}

			if (!headerWasLoaded && serverBytesRead > 0)
			{
				var serverResponseStr = Encoding.UTF8.GetString(responseBytes.ToArray());
				var indexContentLength = serverResponseStr.IndexOf("Content-Length: ", StringComparison.Ordinal);
				if (indexContentLength >= 0)
				{
					var contentLengthStr = "";
					for (
						int j = indexContentLength + 16;
					     serverResponseStr[j] == '0' || serverResponseStr[j] == '1' || serverResponseStr[j] == '2' ||
					     serverResponseStr[j] == '3' || serverResponseStr[j] == '4' || serverResponseStr[j] == '5' ||
					     serverResponseStr[j] == '6' || serverResponseStr[j] == '7' || serverResponseStr[j] == '8' ||
					     serverResponseStr[j] == '9'; 
						j++
					)
					{
						contentLengthStr += serverResponseStr[j];
					}
					contentLength = Convert.ToInt32(contentLengthStr);
				}
				headerWasLoaded = true;
			}
		}
		socket.Close();
		
		// FOR DEBUGGING PURPOSES, UNCOMMENT THIS
		return responseBytes.ToArray();
	}
	

}