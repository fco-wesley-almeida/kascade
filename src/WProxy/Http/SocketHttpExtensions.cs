using System.Net.Sockets;
using WProxy.Extensions;

namespace WProxy.Http;

/// <summary>
/// Extension methods for Socket to facilitate HTTP communication.
/// </summary>
public static class SocketHttpExtensions
{
	/// <summary>
	/// Receives HTTP request headers from the client connected to the socket.
	/// </summary>
	/// <param name="socket">The socket connected to the client.</param>
	/// <returns>Byte array containing the HTTP request headers.</returns>
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
	
	/// <summary>
	/// Sends an HTTP request to a specified URI using the provided socket.
	/// </summary>
	/// <param name="socket">The socket used for communication.</param>
	/// <param name="requestBytes">Byte array containing the HTTP request.</param>
	/// <param name="uri">The URI of the target server.</param>
	/// <returns>Byte array containing the HTTP response.</returns>
	public static byte[] SendHttpRequest(this Socket socket, byte[] requestBytes, Uri uri)
	{
		// Queue to store received bytes constituting the HTTP response. We decided to use queue because insertions are O(1).
		Queue<byte> responseBytes = new(); 
		
		// Connect to the target server specified by the URI
		socket.Connect(uri.Host, uri.Port);
		
		// Send the HTTP request to the server
		socket.Send(requestBytes);
		
		byte lastByte = (byte)'\n';
		int bodyLength = -1;
		int headersLength = -1;
		
		// Loop until both content length and headers length are determined or until data is received
		while(bodyLength == -1 && headersLength == -1 ? socket.Available > 0 || lastByte == '\n' : responseBytes.Count < headersLength + bodyLength)
		{
			// Buffer to hold received bytes. We will allocate (IN STACK) only the memory needed for this transmission (socket.Available). 
			byte[] buffer = new byte[socket.Available];
			
			// Read bytes from the server. This conditional prevents exceptions when the socket is unavailable.
			int serverBytesRead = socket.Available > 0 ? socket.Receive(buffer) : 0;
			
			// Process each byte in the received buffer.
			for (var i = 0; i < buffer.Length; i++)
			{
				var @byte = buffer[i];
				responseBytes.Enqueue(@byte);
				
				// Check for the "Content-Length" header to determine the size of the response body.
				if (bodyLength == -1 && @byte == ':')
				{
					// Verify if the previous bytes form the "Content-Length" header.
					bool isContentLengthHeader =       buffer[i - 14] == 'C' 
					                                && buffer[i - 13] == 'o'
					                                && buffer[i - 12] == 'n'
					                                && buffer[i - 11] == 't'
					                                && buffer[i - 10] == 'e'
					                                && buffer[i - 9]  == 'n'
					                                && buffer[i - 8]  == 't'
					                                && buffer[i - 7]  == '-'
					                                && buffer[i - 6]  == 'L'
					                                && buffer[i - 5]  == 'e'
					                                && buffer[i - 4]  == 'n'
					                                && buffer[i - 3]  == 'g'
					                                && buffer[i - 2]  == 't'
					                                && buffer[i - 1]  == 'h';
					if (isContentLengthHeader)
					{
						string contentLengthStr = "";
						// Here, we will get the number after Content-Length. Remember that a " " splits the number and Content-Length.
						// Example: "Content-Length: 432". This is the reason why we jump 2 chars in the beginning of the loop, not only 1.
						// Remember that at this point the "i" index is pointing to ":' byte. 
						for (int j = i + 2; buffer[j].IsNumeric(); j++) // Once we realise that the number is over, we stop the loop
						{
							contentLengthStr += buffer[j] - '0'; // TODO: surely, there is some way better to convert this number to a real int32.
						}
						bodyLength =  Convert.ToInt32(contentLengthStr);
					}
				}

				// Check for the end of headers to determine the length of the headers section
				if (headersLength == -1 && @byte == '\n')
				{
					// Every HTTP header ends with \r\n\r\n.
					bool isEndHeader = buffer[i - 1] == '\r'
									&& buffer[i - 2] == '\n'
									&& buffer[i - 3] == '\r';
					if (isEndHeader)
					{
						headersLength = i + 1;
					}
				}
			}
			
			// Update the last byte read from the server. This is useful when Content-Length is not informed.
			if (buffer.Length > 0 && serverBytesRead > 0)
			{
				lastByte = buffer[^1];
			}
		}
		socket.Close();
		return responseBytes.ToArray();
	}
}