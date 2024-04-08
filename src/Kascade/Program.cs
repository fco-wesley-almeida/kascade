using Kascade.Core.Logging;
using Kascade.Core.Options;
using Kascade.Core.TCP;
using Kascade.Features.Proxy.Http;

// Receive command line arguments
var options = OptionsCommandLine.FromArgs(args);

// Configure logger
var logger = new List<ILogger>
{
	new LoggerFile(options)
}
.Select(logger => logger.Configure())
.SingleOrDefault(logger => logger.IsEnabled());

if (logger is null)
{
	throw new Exception("Log is not configured.");
}

// Instantiate TCPConnection Listener 
logger.LogInfo($"Starting kaskade listening in {options.Endpoint} for destination {options.Destination}");

try
{
	var tcpListener = new TcpConnectionListener(
		endpoint: options.Endpoint,
		handler: new HttpCallHandler(options.Destination, logger),
		logger: logger
	);
	tcpListener.Listen();
	
	// Listen connections forever
	Console.ReadLine();
}
catch (Exception e)
{
	logger.LogError($"{e.Message} {e.StackTrace}");
	logger.LogInfo("Bye bye");
}

