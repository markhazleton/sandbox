// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;

Console.WriteLine("Hello, World!");
// Replace with your actual API key
string apiKey = "api-key";

// Define the API endpoint URL
string apiUrl = "https://api.openai.com/v1/engines/davinci-codex/completions";

// Create an instance of HttpClient
using (var httpClient = new HttpClient())
{
    // Set the API key in the request headers
    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    // Prompt for user input
    Console.Write("Enter a prompt: ");
    string prompt = Console.ReadLine();

    // Create a request object with the prompt
    var requestData = new
    {
        prompt = prompt,
        max_tokens = 50 // Adjust max_tokens as needed
    };

    // Send a POST request to the API
    var response = await httpClient.PostAsJsonAsync(apiUrl, requestData);

    // Check if the request was successful
    if (response.IsSuccessStatusCode)
    {
        // Read and display the API response
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("API Response:");
        Console.WriteLine(responseBody);
    }
    else
    {
        Console.WriteLine($"API request failed with status code {response.StatusCode}");
        // Optionally handle error responses here
    }
}