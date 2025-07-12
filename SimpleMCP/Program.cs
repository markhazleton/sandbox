using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace MinimalMcpServer;

/// <summary>
/// Tool implementations using static methods
/// </summary>
public static class Tools
{
    /// <summary>
    /// Echoes the message back to the client with "Hello from C#" prefix
    /// </summary>
    /// <param name="message">The message to echo</param>
    /// <returns>The echoed message with prefix</returns>
    [Description("Echoes the message back to the client with 'Hello from C#' prefix")]
    public static string Echo(string message)
    {
        return $"Hello from C#: {message}";
    }

    /// <summary>
    /// Returns the input message in reverse order
    /// </summary>
    /// <param name="message">The message to reverse</param>
    /// <returns>The reversed message</returns>
    [Description("Returns the input message in reverse order")]
    public static string ReverseEcho(string message)
    {
        return new string(message.Reverse().ToArray());
    }

    /// <summary>
    /// Fetches a random joke from JokeAPI
    /// </summary>
    /// <returns>A random programming joke</returns>
    [Description("Fetches a random joke from JokeAPI")]
    public static async Task<string> GetJoke()
    {
        using var client = new HttpClient();
        try
        {
            var response = await client.GetFromJsonAsync<JokeResponse>(
                "https://v2.jokeapi.dev/joke/Programming?safe-mode");

            string joke = response?.Type == "single"
                ? response.Joke ?? "No joke available"
                : $"{response?.Setup}\n{response?.Delivery}";

            return $"JOKE: {joke}";
        }
        catch (Exception ex)
        {
            return $"Error fetching joke: {ex.Message}";
        }
    }

    /// <summary>
    /// Response model for JokeAPI
    /// </summary>
    private class JokeResponse
    {
        public string Type { get; set; } = "";
        public string? Joke { get; set; }
        public string? Setup { get; set; }
        public string? Delivery { get; set; }
    }
}

/// <summary>
/// Connection Manager for SSE connections
/// </summary>
public class McpConnectionManager
{
    private readonly ConcurrentDictionary<string, Channel<string>> _connections = new();

    public Channel<string> CreateConnection(string connectionId)
    {
        var channel = Channel.CreateUnbounded<string>();
        _connections.TryAdd(connectionId, channel);
        return channel;
    }

    public void RemoveConnection(string connectionId)
    {
        if (_connections.TryRemove(connectionId, out var channel))
        {
            channel.Writer.Complete();
        }
    }

    public bool HasConnection(string connectionId)
    {
        return _connections.ContainsKey(connectionId);
    }

    public async Task SendMessage(string connectionId, string message)
    {
        if (_connections.TryGetValue(connectionId, out var channel))
        {
            await channel.Writer.WriteAsync(message);
        }
    }
}

/// <summary>
/// MCP Protocol handler
/// </summary>
public static class McpProtocolHandler
{
    public static object HandleInitialize(int id)
    {
        return new
        {
            jsonrpc = "2.0",
            id,
            result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { }
                },
                serverInfo = new
                {
                    name = "MinimalMcpServer",
                    version = "1.0.0"
                }
            }
        };
    }

    public static object HandleToolsList(int id)
    {
        return new
        {
            jsonrpc = "2.0",
            id,
            result = new
            {
                tools = new object[]
                {
                    new
                    {
                        name = "echo",
                        description = "Echoes the message back to the client with 'Hello from C#' prefix",
                        inputSchema = new
                        {
                            type = "object",
                            properties = new
                            {
                                message = new { type = "string", description = "The message to echo" }
                            },
                            required = new[] { "message" }
                        }
                    },
                    new
                    {
                        name = "reverseecho",
                        description = "Returns the input message in reverse order",
                        inputSchema = new
                        {
                            type = "object",
                            properties = new
                            {
                                message = new { type = "string", description = "The message to reverse" }
                            },
                            required = new[] { "message" }
                        }
                    },
                    new
                    {
                        name = "getjoke",
                        description = "Fetches a random joke from JokeAPI",
                        inputSchema = new
                        {
                            type = "object",
                            properties = new { },
                            required = new string[] { }
                        }
                    }
                }
            }
        };
    }

    public static async Task<object> HandleToolCall(JsonElement request, int id)
    {
        try
        {
            var paramsElement = request.GetProperty("params");
            var toolName = paramsElement.GetProperty("name").GetString();
            var arguments = paramsElement.TryGetProperty("arguments", out var argsElement) ? argsElement : default;

            var result = toolName switch
            {
                "echo" => Tools.Echo(arguments.GetProperty("message").GetString() ?? ""),
                "reverseecho" => Tools.ReverseEcho(arguments.GetProperty("message").GetString() ?? ""),
                "getjoke" => await Tools.GetJoke(),
                _ => throw new Exception($"Unknown tool: {toolName}")
            };

            return new
            {
                jsonrpc = "2.0",
                id,
                result = new
                {
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = result
                        }
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(id, -32602, "Invalid params", ex.Message);
        }
    }

    public static object CreateErrorResponse(int id, int code, string message, string? data = null)
    {
        var error = new
        {
            code,
            message,
            data
        };

        return new
        {
            jsonrpc = "2.0",
            id,
            error
        };
    }
}

/// <summary>
/// Program entry point
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a web application builder for HTTP SSE transport
        var builder = WebApplication.CreateBuilder(args);

        // Configure logging for better integration with MCP clients.
        builder.Logging.AddConsole(consoleLogOptions =>
        {
            consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
        });

        // Add CORS for web clients
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Register HttpClient for API calls
        builder.Services.AddHttpClient();

        // Add singleton for managing SSE connections
        builder.Services.AddSingleton<McpConnectionManager>();

        // Build the web application
        var app = builder.Build();

        // Use CORS
        app.UseCors();

        // Get connection manager
        var connectionManager = app.Services.GetRequiredService<McpConnectionManager>();        // SSE endpoint for MCP (handles both GET for SSE connection and POST for requests)
        app.MapGet("/sse", async (HttpContext context) =>
        {
            context.Response.Headers["Content-Type"] = "text/event-stream";
            context.Response.Headers["Cache-Control"] = "no-cache";
            context.Response.Headers["Connection"] = "keep-alive";
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Cache-Control";

            var connectionId = context.Request.Query["connectionId"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("SSE connection established for connection {ConnectionId}", connectionId);

            try
            {
                var channel = connectionManager.CreateConnection(connectionId);

                // Send initial connection event
                await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(new { type = "connection", connectionId })}\n\n");
                await context.Response.Body.FlushAsync();

                // Keep connection alive and send messages
                await foreach (var message in channel.Reader.ReadAllAsync(context.RequestAborted))
                {
                    await context.Response.WriteAsync($"data: {message}\n\n");
                    await context.Response.Body.FlushAsync();
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("SSE connection cancelled for {ConnectionId}", connectionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in SSE connection for {ConnectionId}", connectionId);
            }
            finally
            {
                connectionManager.RemoveConnection(connectionId);
            }
        });

        // HTTP POST endpoint for MCP requests on SSE endpoint
        app.MapPost("/sse", async (HttpContext context) =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var requestBody = await reader.ReadToEndAsync();
                var connectionId = context.Request.Headers["X-Connection-Id"].FirstOrDefault() ?? "default";

                logger.LogInformation("Received MCP request for connection {ConnectionId}: {Request}", connectionId, requestBody);

                // Parse the MCP request
                var mcpRequest = JsonDocument.Parse(requestBody);
                var method = mcpRequest.RootElement.GetProperty("method").GetString();
                var id = mcpRequest.RootElement.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;

                // Handle MCP requests
                var response = method switch
                {
                    "initialize" => McpProtocolHandler.HandleInitialize(id),
                    "tools/list" => McpProtocolHandler.HandleToolsList(id),
                    "tools/call" => await McpProtocolHandler.HandleToolCall(mcpRequest.RootElement, id),
                    _ => McpProtocolHandler.CreateErrorResponse(id, -32601, "Method not found", $"Unknown method: {method}")
                };

                // For SSE transport, return the response directly (no async messaging)
                return Results.Json(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing MCP request");

                var errorResponse = McpProtocolHandler.CreateErrorResponse(0, -32603, "Internal error", ex.Message);
                return Results.Json(errorResponse);
            }
        });

        // Add endpoint to list available tools
        app.MapGet("/mcp/tools", () =>
        {
            var tools = new object[]
            {
                new {
                    name = "echo",
                    description = "Echoes the message back to the client with 'Hello from C#' prefix",
                    parameters = new { message = new { type = "string", description = "The message to echo" } }
                },
                new {
                    name = "reverseecho",
                    description = "Returns the input message in reverse order",
                    parameters = new { message = new { type = "string", description = "The message to reverse" } }
                },
                new {
                    name = "getjoke",
                    description = "Fetches a random joke from JokeAPI",
                    parameters = new { }
                }
            };

            return Results.Json(new { tools, status = "success" });
        });

        // Add a simple health check endpoint
        app.MapGet("/health", () => Results.Json(new { status = "healthy", transport = "HTTP SSE" }));        // Add a root endpoint with information about the MCP server
        app.MapGet("/", () => Results.Json(new
        {
            name = "MinimalMcpServer",
            version = "1.0.0",
            transport = "HTTP Server-Sent Events",
            endpoints = new
            {
                sse = "/sse (GET for SSE connection, POST for MCP requests)",
                tools = "/mcp/tools",
                health = "/health"
            }
        }));

        // Configure the server to listen on all interfaces
        app.Urls.Add("http://0.0.0.0:3000"); Console.WriteLine("MinimalMcpServer starting with HTTP SSE transport...");
        Console.WriteLine("Server will be available at: http://localhost:3000");
        Console.WriteLine("SSE endpoint: http://localhost:3000/sse (GET for SSE, POST for MCP requests)");
        Console.WriteLine("Tools endpoint: http://localhost:3000/mcp/tools");
        Console.WriteLine("Health check: http://localhost:3000/health");

        // Run the web application. This starts the MCP server with HTTP SSE transport.
        await app.RunAsync();
    }
}
