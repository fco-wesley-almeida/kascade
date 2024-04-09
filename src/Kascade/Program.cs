using System.Reflection;
using Kascade.Core.Logging;
using Kascade.Core.Options;
using Kascade.Core.TCP;
using Kascade.Features.Proxy.Http;

// Receive command line arguments
var options = OptionsCommandLine.FromArgs(args);

// Configure logger
var loggerInstances = Assembly
	.GetExecutingAssembly()
	.GetTypes()
	.Where(type => typeof(ILogChannel).IsAssignableFrom(type) && !type.IsInterface)
	.Select(type =>
	{
		// Find the appropriate constructor
		var constructor = type.GetConstructor(new[] { typeof(IOptions) });
		if (constructor != null)
		{
			// Invoke the constructor with the options parameter
			return (ILogChannel)constructor.Invoke(new object[] { options });
		}
		// Handle if constructor not found (throw exception or return null)
		throw new InvalidOperationException($"Constructor with LogChannelOptions parameter not found in type {type.FullName}");
	});

var logger = loggerInstances
	.Where(logger => logger.IsEnabled())
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
		logChannel: logger
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

