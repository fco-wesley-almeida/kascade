namespace Hermes.Proxy.TCP;

public interface ITcpConnectionHandler
{
	public void AcceptCallback(IAsyncResult asyncResult);
	public TcpProtocol TcpProtocol {get;}
}