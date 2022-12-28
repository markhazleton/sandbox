using CosmosFamily.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace CosmosFamily.Services;

class CosmosService
{
    // The Azure Cosmos DB endpoint for running this sample.
    // private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];

    // The primary key for the Azure Cosmos account.
    // private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

    // The Cosmos client instance
    private readonly CosmosClient cosmosClient;

    // The database we will create
    private Database database;

    // The container we will create.
    private Container container;

    // The name of the database and container we will create
    private readonly string databaseId = "FamilyList";
    private readonly string containerId = "Items";

    public CosmosService()
    {
        var secretReader = new SecretsReader();
        var cosmosDbSettings = secretReader.ReadSection<CosmosDb>("CosmosDb");
        this.databaseId = cosmosDbSettings.DatabaseId;
        this.containerId = cosmosDbSettings.ContainerId;
        this.cosmosClient = new CosmosClient(
            cosmosDbSettings.EndpointUri,
            cosmosDbSettings.PrimaryKey,
            new CosmosClientOptions() { ApplicationName = "CosmosDBFamily" });
    }
    public CosmosService(CosmosClient cosmosClient, Database database, Container container, string databaseId, string containerId)
    {
        this.cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
        this.database = database ?? throw new ArgumentNullException(nameof(database));
        this.container = container ?? throw new ArgumentNullException(nameof(container));
        this.databaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));
        this.containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));
    }

    // <GetStartedDemoAsync>
    /// <summary>
    /// Entry point to call methods that operate on Azure Cosmos DB resources in this sample
    /// </summary>
    public async Task GetStartedDemoAsync()
    {
        await this.CreateDatabaseAsync();
        await this.CreateContainerAsync();
        await this.ScaleContainerAsync();
        await this.AddItemsToContainerAsync();
        await this.QueryItemsAsync();
        await this.ReplaceFamilyItemAsync();
        await this.DeleteFamilyItemAsync();
        await this.DeleteDatabaseAndCleanupAsync();
    }
    // </GetStartedDemoAsync>

    // <CreateDatabaseAsync>
    /// <summary>
    /// Create the database if it does not exist
    /// </summary>
    private async Task CreateDatabaseAsync()
    {
        // Create a new database
        this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        Console.WriteLine($"Created Database: {database.Id}\n");
    }
    // </CreateDatabaseAsync>

    // <CreateContainerAsync>
    /// <summary>
    /// Create the container if it does not exist. 
    /// Specify "/partitionKey" as the partition key path since we're storing family information, to ensure good distribution of requests and storage.
    /// </summary>
    /// <returns></returns>
    private async Task CreateContainerAsync()
    {
        // Create a new container
        this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
        Console.WriteLine($"Created Container: {container.Id}\n");
    }
    // </CreateContainerAsync>

    // <ScaleContainerAsync>
    /// <summary>
    /// Scale the throughput provisioned on an existing Container.
    /// You can scale the throughput (RU/s) of your container up and down to meet the needs of the workload. Learn more: https://aka.ms/cosmos-request-units
    /// </summary>
    /// <returns></returns>
    private async Task ScaleContainerAsync()
    {
        // Read the current throughput
        try
        {
            int? throughput = await this.container.ReadThroughputAsync();
            if (throughput.HasValue)
            {
                Console.WriteLine($"Current provisioned throughput : {throughput.Value}\n");
                int newThroughput = throughput.Value + 100;
                // Update throughput
                await this.container.ReplaceThroughputAsync(newThroughput);
                Console.WriteLine($"New provisioned throughput : {newThroughput}\n");
            }
        }
        catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.BadRequest)
        {
            Console.WriteLine("Cannot read container throughput.");
            Console.WriteLine(cosmosException.ResponseBody);
        }

    }
    // </ScaleContainerAsync>

    // <AddItemsToContainerAsync>
    /// <summary>
    /// Add Family items to the container
    /// </summary>
    private async Task AddItemsToContainerAsync()
    {
        var hazletonFamily = FamilyHelper.CreateHazletonFamily();
        try
        {
            // Read the item to see if it exists.  
            ItemResponse<Family> andersenFamilyResponse = await this.container.ReadItemAsync<Family>(hazletonFamily.Id, new PartitionKey(hazletonFamily.PartitionKey));
            Console.WriteLine($"Item in database with id: {andersenFamilyResponse.Resource.Id} already exists\n");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
            ItemResponse<Family> andersenFamilyResponse = await this.container.CreateItemAsync<Family>(hazletonFamily, new PartitionKey(hazletonFamily.PartitionKey));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine($"Created item in database with id: {andersenFamilyResponse.Resource.Id} Operation consumed {andersenFamilyResponse.RequestCharge} RUs.\n");
        }

        // Create a family object for the Wakefield family
        Family wakefieldFamily = FamilyHelper.CreateWakefieldFamily();

        try
        {
            // Read the item to see if it exists
            ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>(wakefieldFamily.Id, new PartitionKey(wakefieldFamily.PartitionKey));
            Console.WriteLine($"Item in database with id: {wakefieldFamilyResponse.Resource.Id} already exists\n");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Create an item in the container representing the Wakefield family. Note we provide the value of the partition key for this item, which is "Wakefield"
            ItemResponse<Family> wakefieldFamilyResponse = await this.container.CreateItemAsync<Family>(wakefieldFamily, new PartitionKey(wakefieldFamily.PartitionKey));

            // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
            Console.WriteLine($"Created item in database with id: {wakefieldFamilyResponse.Resource.Id} Operation consumed {wakefieldFamilyResponse.RequestCharge} RUs.\n");
        }
    }



    // </AddItemsToContainerAsync>

    // <QueryItemsAsync>
    /// <summary>
    /// Run a query (using Azure Cosmos DB SQL syntax) against the container
    /// Including the partition key value of lastName in the WHERE filter results in a more efficient query
    /// </summary>
    private async Task QueryItemsAsync()
    {
        var sqlQueryText = "SELECT * FROM c WHERE c.PartitionKey = 'Hazleton' ";
        // var sqlQueryText = "SELECT * FROM c ";

        Console.WriteLine($"Running query: {sqlQueryText}\n");

        QueryDefinition queryDefinition = new(sqlQueryText);
        FeedIterator<Family> queryResultSetIterator = this.container.GetItemQueryIterator<Family>(queryDefinition);

        List<Family> families = new();

        while (queryResultSetIterator.HasMoreResults)
        {
            FeedResponse<Family> currentResultSet = await queryResultSetIterator.ReadNextAsync();
            foreach (Family family in currentResultSet)
            {
                families.Add(family);
                Console.WriteLine($"\tRead {family}\n");
            }
        }
    }
    // </QueryItemsAsync>

    // <ReplaceFamilyItemAsync>
    /// <summary>
    /// Replace an item in the container
    /// </summary>
    private async Task ReplaceFamilyItemAsync()
    {
        ItemResponse<Family> wakefieldFamilyResponse = await this.container.ReadItemAsync<Family>("Wakefield.7", new PartitionKey("Wakefield"));
        var itemBody = wakefieldFamilyResponse.Resource;

        // update registration status from false to true
        itemBody.IsRegistered = true;
        // update grade of child
        itemBody.Children[0].Grade = 6;

        // replace the item with the updated content
        wakefieldFamilyResponse = await this.container.ReplaceItemAsync<Family>(itemBody, itemBody.Id, new PartitionKey(itemBody.PartitionKey));
        Console.WriteLine($"Updated Family [{itemBody.LastName},{itemBody.Id}].\n \tBody is now: {wakefieldFamilyResponse.Resource}\n");
    }
    // </ReplaceFamilyItemAsync>

    // <DeleteFamilyItemAsync>
    /// <summary>
    /// Delete an item in the container
    /// </summary>
    private async Task DeleteFamilyItemAsync()
    {
        var partitionKeyValue = "Wakefield";
        var familyId = "Wakefield.7";

        // Delete an item. Note we must provide the partition key value and id of the item to delete
        ItemResponse<Family> wakefieldFamilyResponse = await this.container.DeleteItemAsync<Family>(familyId, new PartitionKey(partitionKeyValue));
        Console.WriteLine($"Deleted Family [{partitionKeyValue},{familyId}]\n");
    }
    // </DeleteFamilyItemAsync>

    // <DeleteDatabaseAndCleanupAsync>
    /// <summary>
    /// Delete the database and dispose of the Cosmos Client instance
    /// </summary>
    private async Task DeleteDatabaseAndCleanupAsync()
    {
        DatabaseResponse databaseResourceResponse = await this.database.DeleteAsync();
        // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

        Console.WriteLine($"Deleted Database: {databaseId}\n");

        //Dispose of CosmosClient
        this.cosmosClient.Dispose();
    }
    // </DeleteDatabaseAndCleanupAsync>
}
