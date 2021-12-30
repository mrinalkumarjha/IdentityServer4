# IdentityServer4
About identity server 4 in dotnet core.

# What is identity server ?

Identity server is framework built on top of oauth2 and openid connect. we can setup centralized authentication as service. different app like console , mobile app, webapp, webapi can use same authentication service. Identity server is open source.
So we can have centralized login at one place for different application.

Identity server also has ability to handle single sign on(SSO). it is open source framework.

Identity server can also manage API access control. pictorial representation of identity server ![image](https://user-images.githubusercontent.com/3676282/128994368-041344f7-4b19-43b8-a984-054a850d81ed.png)


# Some important terms.
  Users : who want to consume resources (ex: api endpoint)
  Client: The medium which user will use to consume resources(API) .. ex: mobile client, laptops, pcs.
  Resources: The api endpoint where actual secured data is stored.
  
# Tools needed fot this course.
  1> Dotnet core sdk 2.1 installed , Check version in commandprompt using dotnet --version
  2> VS code or VS IDE
  3> Chrome plugin: JSON Formatter, postman(this will act as a client)


# What is oAuth2 ?
	oauth2 is open authorization protocol used in data communication between application. It is RFC6749 industry standerd.
	Here is oauth2 protocol abstract flow.
	
	![image](https://user-images.githubusercontent.com/3676282/147556963-7ac23db5-f13c-4e5d-bcb1-d5f8539ae764.png)


	Client: client application  is application that access protected resources on behalf of resource owner. it could be web application, javascript application
	mvc application and mobile application. client must be registered with identity server before it request token.
	
	Client application is classified as public and confidential.

	Resource owner: this is the person  who owns data in system.

	Authorization server: this gives access to token whithin then resource owner authority to authenticated client appllication. in short it manage authorization
	and access.

	Resource server: Resource server allow access to protected resource according to the authorization token ans scope.

	Resources: data resources are data which we want to protect. it must be the unique name. client will access this resource with this name.



# What is OpenID ?
	openid is for authentication. it is extension of oauth2. it is simple authentication layer build on oauth2. it is industry standerd protocol for authentication.

# openid connects endpoints.
	it has main three endpoint.

	1> Authorization Endpoint (/authorize)
	2> Token Endpoint (/token)
	3> UserInfo Endpoint (/userinfo)

# OpenId connect Authentication flow.
	1> Authorization code flow -> "code"
	2> Implicit FLow -> "id_token" or "id_token token"
	3> Hybrid Flow -> "Code id_token" or "code token" or "code id_token token".

 # big picture
 	![image](https://user-images.githubusercontent.com/3676282/147559828-5998fb9f-792a-449a-85cd-9d56e1d95d16.png)

  
  
# Create Identity server project
	Create empty web app project and add nuget package to create identity server.

	Install nuget package IdentityServer4

	Add services.AddIdentityServer(); and  app.UseIdentityServer(); inside startup class.

	Now need to configure client , scopes , resource and test user.

	added following code to add client scope ...

	  services.AddIdentityServer()
                .AddInMemoryClients(new List<Client>())
                .AddInMemoryApiScopes(new List<ApiScope>())
                .AddInMemoryIdentityResources(new List<IdentityResource>())
                .AddTestUsers(new List<TestUser>())
                .AddDeveloperSigningCredential();




  
	Once project runs properly you should able to browse discovery document at follwing url

	https://localhost:5005/.well-known/openid-configuration



# Protect api with identity server using jwt barrier token

	For this we need to add nuget package(Microsoft.AspNetCore.Authentication.JwtBearer) in api project.


	Now after this register jwt package in di container to activate it.
	added following in web api startup 
	   services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:5005";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });


    
	Now we need a valid token to use movie api


# Claim based Authentication with client id claim restriction.
	created seperate identitycontroller to get claims.

	added claim based authentication. need to look more on this

	
            // claim based authentication
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient"));
            });


# Creating mvc client app who will consume api

	created Movies.Client as client app in solution


# Create ui in identity server so that client can login
	For this purpose we will use identity server quick start template.
	goto gitgub idendity  server quickstart project  (https://github.com/IdentityServer/IdentityServer4.Quickstart.UI)

	open existing identity server project in terminal to run power shell script.

	run following command

	iex ((New-Object System.Net.WebClient).DownloadString('https://raw.githubusercontent.com/IdentityServer/IdentityServer4.Quickstart.UI/main/getmain.ps1'))


	This will add ui to our existing identity server project.

	Now register service in di to use ui which we added.

		add this in di container... services.AddControllersWithViews();

		and in middleware add this     app.UseStaticFiles();


     Configure endpoint in middleware

	  app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

	  

	Now if you run your project .. your identity server ui will appear .


	you can login into id server using mrinal as id and mrinal as pass.



# Add authentication layer in Movies.Client app.

	1> in Movies.Client we need to add openid connect nuget package (Microsoft.AspNetCore.Authentication.OpenIdConnect).
	2> Now we can configure openid connect in client app.
		  services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:5005";

                    options.ClientId = "movies_mvc_client";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    //options.Scope.Add("address");
                    //options.Scope.Add("email");
                    //options.Scope.Add("roles");

                    //options.ClaimActions.DeleteClaim("sid");
                    //options.ClaimActions.DeleteClaim("idp");
                    //options.ClaimActions.DeleteClaim("s_hash");
                    //options.ClaimActions.DeleteClaim("auth_time");
                    //options.ClaimActions.MapUniqueJsonKey("role", "role");

                    options.Scope.Add("movieAPI");

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    //options.TokenValidationParameters = new TokenValidationParameters
                    //{
                    //    NameClaimType = JwtClaimTypes.GivenName,
                    //    RoleClaimType = JwtClaimTypes.Role
                    //};
                });
 



# Important lecture 48, 54:(https://www.udemy.com/course/secure-net-microservices-with-identityserver4-oauth2openid/learn/lecture/23231358#overview)


# Setting logout url after logout.
	If we dont add following code for client object after logout app wont redirect back to client site.
	                       //PostLogoutRedirectUris = new List<string>()
                       //{
                       //    "https://localhost:5002/signout-callback-oidc"
                       //},


     now code to logout automatically to client url.
	 inside quickstart > account > accountoptions.cs set following code.

	  public static bool AutomaticRedirectAfterSignOut = true;
		


	

# getting token in client for calling api
	when we will send request to api it require token . so in order to get token here ar esteps..

	1: install identity model nuget package(IdentityModel    by Dominik) in client. this package is oauth2 and openid connect client library.
	2: get token and send request to api with token.
	3: Call api with token.

	# here are 1st method to use protected api 

	
            // 1. "retrieve" our api credentials. This must be registered on Identity Server!
            var apiClientCredentials = new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5005/connect/token",

                ClientId = "movieClient",
                ClientSecret = "secret",

                // This is the scope our Protected API requires. 
                Scope = "movieAPI"
            };

            // creates a new HttpClient to talk to our IdentityServer (localhost:5005)
            var client = new HttpClient();

            // just checks if we can reach the Discovery document. Not 100% needed but..
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if (disco.IsError)
            {
                return null; // throw 500 error
            }

            // 2. Authenticates and get an access token from Identity Server
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            if (tokenResponse.IsError)
            {
                return null;
            }

            // Another HttpClient for talking now with our Protected API
            var apiClient = new HttpClient();

            // 3. Set the access_token in the request Authorization: Bearer <token>
            client.SetBearerToken(tokenResponse.AccessToken);

            // 4. Send a request to our Protected API
            var response = await client.GetAsync("https://localhost:5001/api/movies");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var movieList = JsonConvert.DeserializeObject<List<Movie>>(content);





    # 2nd method using IHttpClientFactory

    For this we will use delegate handler . it will intercept our request and we will get token at one place. this is kind of interceptor same as in angular.


        create httphandler to intercept and attach token.   added AuthenticationDelegatingHandler

        register in startuo


                

            // http operation.


            // 1 create an HttpClient used for accessing the Movies.API

            services.AddTransient<AuthenticationDelegatingHandler>();
            services.AddHttpClient("MovieAPIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/"); // API GATEWAY URL
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            // 2 create an HttpClient used for accessing the IDP
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5005/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

           // services.AddHttpContextAccessor();

            services.AddSingleton(new ClientCredentialsTokenRequest
            {
                Address = "https://localhost:5005/connect/token",
                ClientId = "movieClient",
                ClientSecret = "secret",
                Scope = "movieAPI"
            });

            // http operations



        inside service:


          var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/movies");

            var response = await httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList;




# OIDC authentication flow. LEC-56
    1> Authorization code flow. > code
    2> implicit flow > "id_token" or id_token token
    3> Hybrid flow. > code , "id_token" or code token or "code id_token token"

    We need to update responsetype in client application to code id_token to use hybrid flow.

    in hybrid flow we will get token at the time of login itself. for this we use IHttpContextAccessor

    for hybrid flow we add services.AddHttpContextAccessor();


# Claim based authorization in IS4.

    claims based authorization checks claim and based on claim authorize user.
