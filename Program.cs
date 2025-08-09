using Microsoft.Identity.Client; 
using System.Net.Http.Headers;
using DotNetEnv;

Env.Load();

#region parameters section 
string ClientId = Environment.GetEnvironmentVariable("CLIENT_ID"); 
string Authority = "https://login.microsoftonline.com/organizations"; 
string RedirectURI = "http://localhost";
#endregion



#region Acquire a token for Fabric APIs 
// In this sample we acquire a token for Fabric service with the scopes  
// Workspace.ReadWrite.All and Item.ReadWrite.All 
string[] scopes = new string[] { "https://api.fabric.microsoft.com/Workspace.ReadWrite.All https://api.fabric.microsoft.com/Item.ReadWrite.All" }; 

PublicClientApplicationBuilder PublicClientAppBuilder = 
        PublicClientApplicationBuilder.Create(ClientId) 
        .WithAuthority(Authority) 
        .WithRedirectUri(RedirectURI); 

IPublicClientApplication PublicClientApplication = PublicClientAppBuilder.Build(); 

AuthenticationResult result = await PublicClientApplication.AcquireTokenInteractive(scopes) 
        .ExecuteAsync() 
        .ConfigureAwait(false); 

Console.WriteLine(result.AccessToken); 
#endregion