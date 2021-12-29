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
 



# Important lecture 48:(https://www.udemy.com/course/secure-net-microservices-with-identityserver4-oauth2openid/learn/lecture/23231358#overview)


# Setting logout url after logout.
	If we dont add following code for client object after logout app wont redirect back to client site.
	                       //PostLogoutRedirectUris = new List<string>()
                       //{
                       //    "https://localhost:5002/signout-callback-oidc"
                       //},


     now code to logout automatically to client url.
	 inside quickstart > account > accountoptions.cs set following code.

	  public static bool AutomaticRedirectAfterSignOut = true;
		


	



