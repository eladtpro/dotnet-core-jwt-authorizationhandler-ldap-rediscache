# dotnetcore-jwt-authorizationhandler-ldap-rediscache
asp.net core 3.1 api app, jwt token repository authentication in front of ldap directory controller, with aid of redis-cache, iauthorizationhandler and jwt token repository

# Prerequisites
1. redis-cache - running instanse of distributed cache server: 
https://redislabs.com/blog/redis-on-windows-10/
2. Directory Controller (DC) server for ldap querying and authenticating:
https://www.nuget.org/packages/Novell.Directory.Ldap.NETStandard/4.0.0-alpha1
3. aspnet core 3.1:
https://dotnet.microsoft.com/download/dotnet-core/3.1

# Authorization
1. Policy-based authorization in ASP.NET Core:
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-3.1
2. AuthorizationHandler<TRequirement> Class:
https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationhandler-1?view=aspnetcore-3.1
3. 
