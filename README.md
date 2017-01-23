# authServer

Steps to run solution:
* Install .Net Core v.1.1.0 https://www.microsoft.com/net/core#macos
* Clone https://github.com/yevheniiyankovyi/authServer 
* Switch to branch: demo and pull latest changes
* Navigate to ~\authServer\IdentityServer\src
* Execute in cmd: dotnet run

* Open in web browser http://localhost:5000/.well-known/openid-configuration and you should be able to see discovery document
* More information and samples about IdentityServer4 you can find here: http://docs.identityserver.io/en/release

* Now you should be able to get token
```JSON 
POST http://localhost:5000/connect/token
HEADERS
Content-Type:application/x-www-form-urlencoded
Accept:application/json
BODY
client_id:demo
grant_type:password
username:admin@mail.com
password:qweQWE123!
client_secret:secret
scope:users
```

* Now you can validate token
```JSON
POST http://localhost:5000/connect/introspect
HEADERS
Authorization:Basic dXNlcnM6c2VjcmV0
BODY
token:<token>
```

* As far as you have valid token with admin rights you can add new user

```JSON
POST http://localhost:5000/api/profile
HEADERS
Content-Type:application/json
Authorization:Bearer <token>
BODY
{
	"email": "<email>",
	"password": "<password>",
	"firstname": "<f_name>",
	"lastname": "<l_name>",
	"address": "<address>",
	"isAdmin": true
}
```

* After new user is create you can try to get new token for new createt user
