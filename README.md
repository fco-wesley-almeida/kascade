# Kascade

Kascade is a lightweight proxy server implementation in C#/.NET. It provides functionalities for handling TCP connections and HTTP requests.

## Features

- TCP connection handling
- HTTP request processing
- Proxying HTTP requests to target servers
- Asynchronous I/O operations

## Components

Kascade consists of the following main components:

- **Kascade.TCP:** Provides classes for TCP connection handling and listener setup.
- **Kascade.Http:** Contains classes for processing HTTP requests and responses.
- **Kascade.Extensions:** Contains extension methods used within the project.

## Usage

To use Kascade, you should pass as argument the endpoint where the proxy will listen and the destination.

OBS: for now, we only have support for HTTP. In the future versions, we'll have support for SSL/TNS. 

Example:

```shellscript
kaskade --endpoint=0.0.0.0:8083 --destination=http://193.298.12.4:9090 --log-channel=console
