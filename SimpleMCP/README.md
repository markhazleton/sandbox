# MinimalMcpServer

A minimal Model Context Protocol (MCP) server implementation built with .NET 9, following the [Microsoft Learn tutorial](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server).

**Now supports both STDIO and HTTP Server-Sent Events (SSE) transport!**

## Overview

This project provides a basic MCP server that exposes simple echo tools. The AI model can invoke these tools as necessary to generate responses to user prompts. The server integrates with GitHub Copilot and other MCP-compatible clients.

The server can run in two modes:

- **STDIO Transport**: Traditional standard input/output for direct integration with MCP clients
- **HTTP SSE Transport**: Web-based Server-Sent Events for HTTP-based clients and web applications

## Features

- **Echo Tool**: Echoes messages back to the client with a "Hello from C#" prefix
- **ReverseEcho Tool**: Returns the input message in reverse order
- **GetJoke Tool**: Fetches random jokes from the [JokeAPI](https://jokeapi.dev/)
- **Dual Transport Support**: Supports both STDIO and HTTP SSE transports
- **RESTful API**: HTTP endpoints for tool invocation and server status
- **Host Builder Integration**: Uses Microsoft.Extensions.Hosting for dependency injection and logging
- **Modern ASP.NET Core**: Built with .NET 9 and ASP.NET Core for HTTP transport

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio Code (recommended) with GitHub Copilot extension
- GitHub Copilot extension for VS Code

### Building the Project

```bash
dotnet build
```

### Running with HTTP SSE Transport (New!)

```bash
dotnet run
```

The server will start on `http://localhost:3000` with the following endpoints:

- **SSE Endpoint**: `http://localhost:3000/sse` - Server-Sent Events for real-time communication
- **Tools API**: `http://localhost:3000/mcp/tools` - List available tools
- **Tool Invocation**: `http://localhost:3000/mcp/tools/{toolName}` - Invoke specific tools via POST
- **Health Check**: `http://localhost:3000/health` - Server status
- **Server Info**: `http://localhost:3000/` - Server information and endpoints

#### Example HTTP Tool Invocation

```bash
# Echo tool
curl -X POST http://localhost:3000/mcp/tools/echo \
  -H "Content-Type: application/json" \
  -d '{"arguments":{"message":"Hello World!"}}'

# Get a joke
curl -X POST http://localhost:3000/mcp/tools/getjoke \
  -H "Content-Type: application/json" \
  -d '{"arguments":{}}'
```

### Running with STDIO Transport (Original)

```bash
dotnet run
```

### Testing with GitHub Copilot

1. Ensure the `.vscode/mcp.json` configuration file is present
2. Open GitHub Copilot in VS Code and switch to agent mode
3. Verify your MinimalMcpServer appears in the available tools
4. Test with prompts like: "Reverse the following: 'Hello, minimal MCP server!'"

## Project Structure

- `Program.cs` - Main entry point with MCP server configuration and tool definitions
- `MinimalMcpServer.csproj` - Project configuration with MCP dependencies
- `.vscode/mcp.json` - VS Code MCP server configuration

## MCP Tools

### Echo

- **Description**: Echoes the message back to the client with "Hello from C#" prefix
- **Parameter**: `message` (string) - The message to echo

### ReverseEcho

- **Description**: Returns the input message in reverse order
- **Parameter**: `message` (string) - The message to reverse

### GetJoke

- **Description**: Fetches a random joke from JokeAPI
- **Parameters**: None
- **Returns**: A random joke (either single-line or setup/delivery format)

## Dependencies

- `ModelContextProtocol` (0.2.0-preview.3) - Official C# SDK for MCP
- `Microsoft.Extensions.Hosting` (9.0.6) - Generic host builder and services
- `Microsoft.Extensions.Http` (9.0.6) - HttpClient factory and dependency injection

## Development

This project follows modern C# best practices:

- Top-level programs with minimal hosting
- File-scoped namespaces  
- Nullable reference types enabled
- Implicit usings enabled
- Async/await patterns for asynchronous operations
- Attribute-based tool registration with MCP

## Configuration

The server supports both transport modes:

### HTTP SSE Transport (Default)

By default, the server runs with HTTP SSE transport on port 3000. This allows:

- Web-based client integration
- RESTful API access to tools
- Real-time communication via Server-Sent Events
- Cross-origin requests (CORS enabled)

### STDIO Transport (Legacy)

The server is also configured in `.vscode/mcp.json` to run via `dotnet run` and communicate using stdio transport for backward compatibility. This allows GitHub Copilot to automatically discover and use the server's tools.

The configuration file includes both options:

- `MinimalMcpServer`: HTTP SSE transport at `http://localhost:3000/sse`
- `MinimalMcpServer-STDIO`: Traditional STDIO transport via dotnet run

## Contributing

1. Follow the coding guidelines defined in the project instructions
2. Write clean, maintainable code with appropriate error handling
3. Add XML documentation comments for public APIs
4. Test your changes with GitHub Copilot

## Related Resources

- [Microsoft Learn: Build MCP Server Tutorial](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server)
- [Model Context Protocol Documentation](https://modelcontextprotocol.io/)
- [GitHub Copilot Extensions](https://github.com/features/copilot/extensions)

This project is open source. Please add your preferred license.

## Resources

- [Model Context Protocol Documentation](https://modelcontextprotocol.io/)
- [.NET Documentation](https://docs.microsoft.com/dotnet/)
