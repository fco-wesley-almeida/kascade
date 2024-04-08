using System.Net.Sockets;

namespace Kascade.Core.Extensions;

public static class SocketExtensions
{
	public static bool TryConnect(this Socket socket, Uri uri, out Exception? exception)
	{
		try
		{
			socket.Connect(uri.Host, uri.Port);
			exception = null;
			return true;
		}
		catch (Exception e)
		{
			exception = e;
			return false;
		}
	}
}