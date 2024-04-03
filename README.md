# WProxy

WProxy is a lightweight proxy server implementation in C#/.NET. It provides functionalities for handling TCP connections and HTTP requests.

## Features

- TCP connection handling
- HTTP request processing
- Proxying HTTP requests to target servers
- Asynchronous I/O operations

## Components

WProxy consists of the following main components:

- **WProxy.TCP:** Provides classes for TCP connection handling and listener setup.
- **WProxy.Http:** Contains classes for processing HTTP requests and responses.
- **WProxy.Extensions:** Contains extension methods used within the project.

## Usage

To use WProxy in your project:

1. Include the necessary WProxy namespace(s) in your code.
2. Create an instance of `TcpConnectionListener` with the desired endpoint and `ITcpConnectionHandler` implementation.
3. Implement the `ITcpConnectionHandler` interface to handle incoming TCP connections and HTTP requests.

Example:

```csharp
// Set up listener
var endpoint = new IPEndPoint(IPAddress.Any, 8080);
var handler = new HttpCallHandler(new Uri("http://example.com"));
var listener = new TcpConnectionListener(endpoint, handler);

// Start listening
listener.Listen();
