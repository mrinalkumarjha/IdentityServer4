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

  
# Steps to setup proj
  > Open vs and create project with blank solution.
  > Create webapi project by right clicking on solution > add new project > webapi core 2.1 proj 
  > Create IdentityServer project (we can create identity project as seperate solution also)
  


