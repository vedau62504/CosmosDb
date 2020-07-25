using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace CosmosConsoleApp1
{
   
    public class Program
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

        public static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Beginning operations... \n");
                Program p = new Program();
                await p.GetStartedDemoAsync();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: { 1}", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        public async Task GetStartedDemoAsync()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
        }


    }
}
