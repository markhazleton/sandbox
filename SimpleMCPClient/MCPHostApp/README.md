# MCP Client Application

This is a minimal Model Context Protocol (MCP) client built with .NET that demonstrates how to connect to an MCP server and interact with it using Azure OpenAI.

## Prerequisites

- [.NET 8.0 SDK or higher](https://dotnet.microsoft.com/download)
- Azure OpenAI subscription and deployment
- An MCP server (you can use the [Build a minimal MCP server](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server) quickstart)

## Configuration

1. **Update `appsettings.json`** with your specific values:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://YOUR-AZURE-OPENAI-ENDPOINT.openai.azure.com",
    "ModelDeploymentName": "gpt-4o"
  },
  "MCPServer": {
    "Command": "dotnet run",
    "ProjectPath": "PATH-TO-YOUR-MCP-SERVER-PROJECT",
    "Name": "Minimal MCP Server"
  }
}
```

2. **Azure Authentication**: This application uses `DefaultAzureCredential` which supports multiple authentication methods:
   - Azure CLI: Run `az login` in your terminal
   - Visual Studio: Sign in through Visual Studio
   - Environment variables: Set `AZURE_CLIENT_ID`, `AZURE_CLIENT_SECRET`, `AZURE_TENANT_ID`
   - Managed Identity: Automatically used when running on Azure

## Setup Steps

1. **Clone or download this project**

2. **Install dependencies**:

   ```bash
   dotnet restore
   ```

3. **Configure your Azure OpenAI endpoint**:
   - Update the `Endpoint` in `appsettings.json` with your Azure OpenAI resource endpoint
   - Ensure the `ModelDeploymentName` matches your deployed model

4. **Set up MCP Server**:
   - Create an MCP server following the [MCP server quickstart](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server)
   - Update the `ProjectPath` in `appsettings.json` to point to your MCP server project

5. **Authenticate with Azure**:

   ```bash
   az login
   ```

## Running the Application

```bash
dotnet run
```

The application will:

1. Connect to your Azure OpenAI endpoint
2. Start and connect to your MCP server
3. List available tools from the MCP server
4. Enter a conversational loop where you can interact with the AI using the MCP tools

## Example Usage

Once the application is running, you can type prompts like:

```
Prompt: Reverse the following: "Hello, minimal MCP server!"
```

The AI will use the available MCP tools to process your request and provide a response.

Type `exit` or `quit` to stop the application.

## Features

- **Secure Authentication**: Uses Azure Managed Identity and DefaultAzureCredential
- **Configuration-driven**: Easy configuration through `appsettings.json`
- **Error Handling**: Comprehensive error handling and user-friendly error messages
- **Streaming Responses**: Real-time streaming of AI responses
- **Tool Integration**: Automatic integration with MCP server tools

## Troubleshooting

1. **Authentication Issues**:
   - Ensure you're logged into Azure CLI: `az login`
   - Verify your Azure subscription has access to the OpenAI resource
   - Check that your Azure OpenAI endpoint URL is correct

2. **MCP Server Connection Issues**:
   - Verify the MCP server project path is correct
   - Ensure the MCP server can be built and run successfully
   - Check that the MCP server is properly configured

3. **Package Issues**:
   - Try clearing NuGet cache: `dotnet nuget locals all --clear`
   - Restore packages: `dotnet restore`

## Dependencies

- Azure.AI.OpenAI (prerelease)
- Azure.Identity
- Microsoft.Extensions.AI
- Microsoft.Extensions.AI.OpenAI (prerelease)
- ModelContextProtocol (prerelease)
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Configuration.EnvironmentVariables

## Learn More

- [Microsoft Learn: Build a minimal MCP client](https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-client)
- [Model Context Protocol Documentation](https://modelcontextprotocol.io/)
- [Azure OpenAI Service Documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/openai/)
