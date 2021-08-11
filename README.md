# IdentityServer4
About identity server 4 in dotnet core.

# What is identity server ?

Identity server is framework built on top of oauth2 and openid connect. we can setup centralized authentication as service. different app like console , mobile app, webapp, webapi can use same authentication service. Identity server is open source.
So we can have centralized login at one place for different application.

Identity server also has ability to handle single sign on(SSO)

Identity server can also manage API access control. pictorial representation of identity server ![image](https://user-images.githubusercontent.com/3676282/128994368-041344f7-4b19-43b8-a984-054a850d81ed.png)


# Some important terms.
  Users : who want to consume resources (ex: api endpoint)
  Client: The medium which user will use to consume resources(API) .. ex: mobile client, laptops, pcs.
  Resources: The api endpoint where actual secured data is stored.
  
# Tools needed fot this course.
  1> Dotnet core sdk 2.1 installed , Check version in commandprompt using dotnet --version
  2> VS code or VS IDE
  3> Chrome plugin: JSON Formatter, postman(this will act as a client)
  
# Steps to setup proj
  > Open vs and create project with blank solution.
  > Create webapi project by right clicking on solution > add new project > webapi core 2.1 proj 
  > Create IdentityServer project (we can create identity project as seperate solution also)
  > Create console client project
  
# Install Identity server package in identity server project.

	> Install identityserver4 2.2.0 from nuget package manager. (project url: https://github.com/IdentityServer/IdentityServer4)
	 After this installation we can add middleware for identityserver.

	>

# Launch identity server:
	run identity server project
	open link : http://localhost:5000/.well-known/openid-configuration
	this will bring all openid configuration. here are some important property. this is also called discovery file.

	issuer : this is identity service.
	token_endpoint : this is endpoint for token
	scopes_supported : this list all apis supported
	grant_types_supported : grant type supported 

	ex:
	    "grant_types_supported": [
		"authorization_code",
		"client_credentials",
		"refresh_token",
		"implicit"
		]

#  Invoke token endpoint

	copy url from discovery file : http://localhost:5000/connect/token
	make post call to token endpoint.

	in postman 
	set authorization type as Basic
	username : client
	password: secret

	in body
	grant_type : client_credentials
	scope : bankOfDotNetApi   (scope of this client) 


# Securing webapi using identityserver 4

	For this we need to add accesstoken validation package in webapi project to validate access token
	go to nuget package manager > search identityserver4.accesstokenvalidation > install version 2.6.0

	add following code in dependencu container inside startup

	services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "bankOfDotNetApi";
                }
                );


	Now add [Authorize] attribute in controller. now without authentication no client would be able to make
	request.

# sequence to launch all app

	first identity server > api > then any client

# Create console client project

	this project is client and will consume webapi. 

	add nuget package IdentityModel. this is OpenID Connect & OAuth 2.0 client library.
	add version 3.9

# Code to consume api from client console app.

	class Program
    {
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

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
    }


# 




