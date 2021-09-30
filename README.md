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


# Grant type:
	In OAuth 2.0, the term “grant type” refers to the way an application gets an access token from identity server. ... 
    Each grant type is optimized for a particular use case, whether that's a web app, a native app, 
    a device without the ability to launch a web browser, or server-to-server applications.

	grant type is a way by which clients(mobile, pc, browser, native mobile app) can communicate with resources.
	
    Here are some popular grant types:
	
	1> Client Credential: this can be used for server to server communication or trusted communication like intranet.
        for ex if client(console app) want to 
        access resource or api to api communication. this method should not be used for client (web application like browser) because it use clientid
        and secret which browser can store it. so no user involve here.
        we dont want to expose client credential to public.
        for ex: in this app client console app want to access resources so we have used Client credential as grant type.

    2> Resource owner password: resource owner can be a user. user will use browser or any client to access resource.
        resource owner has capability of granting access to resources which is protected by identity server.
        in this case client not only need to send client id to server but client would also send resource owner 
        userid and pass.

        in this case user is involved. this can be used if we know client is capable of storing user credential(trusted first party).
        like spa, js, native app .

    3> Authorization Code: situation when we need to give authentication using google or fb or any third party.
        user involved, webapp server side, third party native app.
        
    4> Implicit: in this case client redirects user to identityserver login page, where user enter his id , pass
        as soon as user login. identity server 4 will provide a consent page asking do u approve this client for making 
        calls to resources. once consent is provided to client by user, access token is provided and by which browser 
        would be able to access resources.
        this is optimized for server side app, spa, user is involved here.

    5> Hybrid: it is combination of implicit and authorization code. user is envolve here. can used for web app, spa, native app.
            this is best choice .


# Resource owner password flow added:
    MainResourceOwnerAsync is method inside console app which has code to generate token with resource owner flow.

# Implicit flow setup:

    i have created seperate identity server project named (ImplicitFlow.IdentityServer)
    for implicit flow. This is ui based identity server.





# Implicit flow client app:

    I have created seperate implicit flow mvc client app(BankofDotnet.MvcClient).

    configure mvc client to use identityServer.


