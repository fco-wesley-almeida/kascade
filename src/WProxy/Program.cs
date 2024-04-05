using WProxy.Options;
using WProxy.Proxy.Http;
using WProxy.Proxy.TCP;

var options = OptionsCommandLine.FromArgs(args);

Console.WriteLine($"Starting wproxy listening in {options.Endpoint} for destination {options.Destination}");

var tcpListener = new TcpConnectionListener(
	endpoint: options.Endpoint, 
	handler: new HttpCallHandler(options.Destination)
);

tcpListener.Listen();
Console.ReadLine();