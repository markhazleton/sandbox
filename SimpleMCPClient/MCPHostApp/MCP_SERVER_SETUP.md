# Example MCP Server Setup

Since the MCP client requires an MCP server to connect to, here's a quick example of how to set up a simple MCP server for testing.

## Option 1: Create a Simple Test MCP Server

Create a new folder for your MCP server:

```bash
mkdir TestMCPServer
cd TestMCPServer
dotnet new console
dotnet add package ModelContextProtocol --prerelease
```

Replace the Program.cs content with:

```csharp
using ModelContextProtocol.Server;

var server = new McpServer()
    .WithTool("reverse_echo", "Reverses the input text", async (string text) =>
    {
        return new string(text.Reverse().ToArray());
    })
    .WithTool("uppercase", "Converts text to uppercase", async (string text) =>
    {
        return text.ToUpper();
    });

await server.RunAsync();
```

## Option 2: Use the Official Microsoft Tutorial

Follow the complete MCP server tutorial:
<https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server>

## Updating Your Configuration

Once you have your MCP server ready, update the `appsettings.json` in your MCP client:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://YOUR-AZURE-OPENAI-ENDPOINT.openai.azure.com",
    "ModelDeploymentName": "gpt-4o"
  },
  "MCPServer": {
    "Command": "dotnet run",
    "ProjectPath": "C:\\path\\to\\your\\TestMCPServer",
    "Name": "Test MCP Server"
  }
}
```

Replace `C:\\path\\to\\your\\TestMCPServer` with the actual path to your MCP server project.
