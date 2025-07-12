using Azure.AI.OpenAI;
using Azure.Identity;
using Azure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using ModelContextProtocol.Client;

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

try
{    // Get configuration values
    var azureOpenAIEndpoint = configuration["AzureOpenAI:Endpoint"] ??
        throw new InvalidOperationException("Azure OpenAI endpoint not configured. Please update appsettings.json with your Azure OpenAI endpoint.");

    var apiKey = configuration["AzureOpenAI:ApiKey"];
    var deploymentName = configuration["AzureOpenAI:DeploymentName"] ??
        configuration["AzureOpenAI:ModelDeploymentName"] ?? "gpt-4o";

    var mcpServerProjectPath = configuration["MCPServer:ProjectPath"] ??
        throw new InvalidOperationException("MCP Server project path not configured. Please update appsettings.json with the path to your MCP server project.");

    var mcpServerCommand = configuration["MCPServer:Command"] ?? "dotnet run";
    var mcpServerName = configuration["MCPServer:Name"] ?? "MCP Server";

    Console.WriteLine("Initializing MCP Client...");
    Console.WriteLine($"Azure OpenAI Endpoint: {azureOpenAIEndpoint}");
    Console.WriteLine($"Model: {deploymentName}");
    Console.WriteLine($"MCP Server: {mcpServerName}");
    Console.WriteLine();

    // Create an IChatClient using Azure OpenAI with API Key or Managed Identity
    AzureOpenAIClient azureClient;
    if (!string.IsNullOrEmpty(apiKey))
    {
        Console.WriteLine("Using API Key authentication");
        azureClient = new AzureOpenAIClient(new Uri(azureOpenAIEndpoint), new Azure.AzureKeyCredential(apiKey));
    }
    else
    {
        Console.WriteLine("Using Managed Identity authentication");
        azureClient = new AzureOpenAIClient(new Uri(azureOpenAIEndpoint), new DefaultAzureCredential());
    }

    IChatClient client =
        new ChatClientBuilder(azureClient.GetChatClient(deploymentName).AsIChatClient())
        .UseFunctionInvocation()
        .Build();

    Console.WriteLine("Connecting to MCP server...");

    // Create the MCP client
    // Configure it to start and connect to your MCP server.
    IMcpClient mcpClient = await McpClientFactory.CreateAsync(
        new StdioClientTransport(new()
        {
            Command = mcpServerCommand,
            Arguments = ["--project", mcpServerProjectPath],
            Name = mcpServerName,
        }));


    // List all available tools from the MCP server.
    Console.WriteLine("Available tools:");
    IList<McpClientTool> tools = await mcpClient.ListToolsAsync();

    if (tools.Count == 0)
    {
        Console.WriteLine("No tools available from the MCP server.");
        Console.WriteLine("Make sure your MCP server is running and properly configured.");
        return;
    }

    foreach (McpClientTool tool in tools)
    {
        Console.WriteLine($"  - {tool}");
    }
    Console.WriteLine();
    Console.WriteLine("Type 'exit' or 'quit' to stop the application.");
    Console.WriteLine();

    // Conversational loop that can utilize the tools via prompts.
    List<ChatMessage> messages = [];
    while (true)
    {
        Console.Write("Prompt: ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
            continue;

        if (input.ToLower() == "exit" || input.ToLower() == "quit")
            break;

        messages.Add(new(ChatRole.User, input));

        try
        {
            List<ChatResponseUpdate> updates = [];
            await foreach (ChatResponseUpdate update in client
                .GetStreamingResponseAsync(messages, new() { Tools = [.. tools] }))
            {
                Console.Write(update);
                updates.Add(update);
            }
            Console.WriteLine();
            Console.WriteLine();

            messages.AddMessages(updates);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing request: {ex.Message}");
            Console.WriteLine();
        }
    }

    Console.WriteLine("Goodbye!");
}
catch (Exception ex)
{
    Console.WriteLine($"Application error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Please check your configuration and ensure:");
    Console.WriteLine("1. Azure OpenAI endpoint is correct in appsettings.json");
    Console.WriteLine("2. You have proper Azure credentials configured");
    Console.WriteLine("3. MCP server project path is correct");
    Console.WriteLine("4. MCP server is accessible and running");

    Environment.Exit(1);
}
