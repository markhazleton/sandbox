using CosmosFamily.Services;
using Microsoft.Azure.Cosmos;


try
{
    Console.WriteLine("Beginning operations...\n");
    CosmosService p = new();
    await p.GetStartedDemoAsync();
}
catch (CosmosException de)
{
    Exception baseException = de.GetBaseException();
    Console.WriteLine($"{de.StatusCode} error occurred: {de}");
}
catch (Exception e)
{
    Console.WriteLine($"Error: {e}");
}
finally
{
    Console.WriteLine("End of demo, press any key to exit.");
    Console.ReadKey();
}
