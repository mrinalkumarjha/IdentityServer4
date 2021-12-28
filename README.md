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


# What is oAuth2 ?
	oauth2 is open authorization protocol used in data communication between application. It is RFC6749 industry standerd.
	Here is oauth2 protocol abstract flow.



# What is OpenID ?
	openid is for authentication.


  
# Steps to setup proj
  > Open vs and create project with blank solution.
  > Create webapi project by right clicking on solution > add new project > webapi core 2.1 proj 
  > Create IdentityServer project (we can create identity project as seperate solution also)
  

