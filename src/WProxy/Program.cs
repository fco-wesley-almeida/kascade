using System.Net;
using WProxy;
using WProxy.Http;
using WProxy.TCP;

var tcpListener = new TcpConnectionListener(
	endpoint: new(IPAddress.Any, 8083), 
	// handler: new HttpCallHandler(new("http://0.0.0.0:8080"))
	handler: new HttpCallHandler(new("http://135.148.2.37:8883"))
);
tcpListener.Listen();
Console.ReadLine();