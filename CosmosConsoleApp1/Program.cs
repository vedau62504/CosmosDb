using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace CosmosConsoleApp1
{
   
    class Program
    {
        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = "https://cognitiveservices.documents.azure.com:443/";
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = "aJsPFDFxwmFUcIaTyPBm7toFKwEh6J80GL4P3D4jgmU0TdsNqYkLyIUS2yxDTDfoY4yWXLeFrOSTmMgU3JM5ig==";

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create
        private Container container;

        // The name of the database and container we will create
        private string databaseid = "FamilyDatabase";
        private string containerid = "FamilyContainer";

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations...");
                Program p = new Program();
                await p.GetStartedDemoAsync();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: { 1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        public async Task GetStartedDemoAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();
            await this.AddItemstoContainerAsync();
        }

        // Create the database if it does not exist
        private async Task CreateDatabaseAsync()
        {
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseid);
            Console.WriteLine("Create Database:{0}\n", this.database.Id);
        }

        // Create the container if it does not exist.

        private async Task CreateContainerAsync()
        {
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerid, "/LastName");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        // Add family items to the container
        private async Task AddItemstoContainerAsync()
        {
            // Create a family object for the Le family
            Family leFamily = new Family
            {
                Id = "Le.1",
                LastName = "Le",
                Parents = new Parent[]
                {
                    new Parent {FamilyName = "Le", FirstName = "Tho"},
                    new Parent {FamilyName = "Nguyen", FirstName = "Anny"}
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Le",
                        FirstName = "Olivia",
                        Gender = "female",
                        Grade = 2,
                        Pets = new Pet[]
                        {
                            new Pet {GivenName = "Fluffy"}
                        }
                    }
                },
                Address = new Address { State = "VA", County = "Loudoun", City = "Chantilly" },
                IsRegistered = false
            };

            try
            {
                // Read the item to see if it exists
                ItemResponse<Family> leFamilyResponse = await this.container.ReadItemAsync<Family>(leFamily.Id, new PartitionKey(leFamily.LastName));
                Console.WriteLine("Item in database with id: {0} already exists\n", leFamilyResponse.Resource.Id);
            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Le Family. Note we provide the value of the partition key for this item, which is "Le"
                ItemResponse<Family> leFamilyResponse = await this.container.CreateItemAsync<Family>(leFamily, new PartitionKey(leFamily.LastName));

                // Note that after creating the item, we can access the body of the item with the Resource property of the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n.", leFamilyResponse.Resource.Id, leFamilyResponse.RequestCharge);

            }
            // Create a family object for the Nguyen family

            Family nguyenFamily = new Family
            {
                Id = "Nguyen.2",
                LastName = "Nguyen",
                Parents = new Parent[]
                {
                    new Parent {FamilyName = "Nguyen", FirstName = "Tuoi"},
                    new Parent {FamilyName = "Tran", FirstName = "Lunar"}
                },

                Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Jake",
                        FirstName = "Le",
                        Gender = "male",
                        Grade = 1,
                        Pets = new Pet[]
                        {
                            new Pet {GivenName = "Goofy"},
                            new Pet {GivenName = "Monster"}
                        }
                    },
                    new Child
                    {
                        FamilyName = "Miller",
                        FirstName = "Lisa",
                        Gender = "female",
                        Grade = 12
                    }
                },
                Address = new Address { State = "MD", County = "PG", City = "Beltsville" },
                IsRegistered = true
            };

            try
            {
                // Read the item to see if it exists
                ItemResponse<Family> nguyenFamilyResponse = await this.container.ReadItemAsync<Family>(nguyenFamily.Id, new PartitionKey(nguyenFamily.LastName));
                Console.WriteLine("Item in database with id: {0} already exists\n", nguyenFamilyResponse.Resource.Id);

            }
            catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an iten in the container representing in the Nguyen Family.
                ItemResponse<Family> nguyenFamilyResponse = await this.container.CreateItemAsync<Family>(nguyenFamily, new PartitionKey(nguyenFamily.LastName));
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs. \n", nguyenFamilyResponse.Resource.Id, nguyenFamilyResponse.RequestCharge);

            }
        }
    }
}
