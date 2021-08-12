using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BankOfDotNet.ConsoleClient
{
    class Program
    {
        static void Main(string[] args) 
        {
            MainResourceOwnerAsync().GetAwaiter().GetResult();
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            // discover all endpoint using metadata of identity server

            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if(disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // grab bearer token

            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");

            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("bankOfDotNetApi");

            if(tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // code to consume customer api.

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            // create customer obj
            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(
                    new {Id = 10, FirstName = "Mtest", LastName = "jha"}
                    ), Encoding.UTF8, "application/json" );

            var createCustomerResponse = await client.PostAsync("http://localhost:33896/api/customers",
                customerInfo);

            if(!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(createCustomerResponse.StatusCode);
            }


            // get all customer
            var GetCustomerResponse = await client.GetAsync("http://localhost:33896/api/customers");
            if(!GetCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(GetCustomerResponse.StatusCode);
            }
            else
            {
                var content = await GetCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));

            }

            Console.Read();


        }

        private static async Task MainResourceOwnerAsync()
        {
            // discover all endpoint using metadata of identity server

            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // grab bearer token using resource flow

            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("mrinal", "password", "bankOfDotNetApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // code to consume customer api.

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            // create customer obj
            var customerInfo = new StringContent(
                JsonConvert.SerializeObject(
                    new { Id = 10, FirstName = "Mtest", LastName = "jha" }
                    ), Encoding.UTF8, "application/json");

            var createCustomerResponse = await client.PostAsync("http://localhost:33896/api/customers",
                customerInfo);

            if (!createCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(createCustomerResponse.StatusCode);
            }


            // get all customer
            var GetCustomerResponse = await client.GetAsync("http://localhost:33896/api/customers");
            if (!GetCustomerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine(GetCustomerResponse.StatusCode);
            }
            else
            {
                var content = await GetCustomerResponse.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));

            }

            Console.Read();


        }
    }
}
