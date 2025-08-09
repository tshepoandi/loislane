using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using DotNetEnv;
using System.Text.Json;

Env.Load();

#region parameters section
string ClientId = Environment.GetEnvironmentVariable("CLIENT_ID");
string Authority = "https://login.microsoftonline.com/organizations";
string RedirectURI = "http://localhost";
#endregion

#region Acquire a token for Fabric APIs
string[] scopes = new string[]
{
    "https://api.fabric.microsoft.com/Workspace.ReadWrite.All",
    "https://api.fabric.microsoft.com/Item.ReadWrite.All"
};

var PublicClientAppBuilder = PublicClientApplicationBuilder.Create(ClientId)
    .WithAuthority(Authority)
    .WithRedirectUri(RedirectURI);

var PublicClientApplication = PublicClientAppBuilder.Build();

var result = await PublicClientApplication.AcquireTokenInteractive(scopes)
    .ExecuteAsync()
    .ConfigureAwait(false);

Console.WriteLine("Access token acquired.");

#endregion

#region Call Fabric API to list workspaces and items
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", result.AccessToken);

// 1️⃣ List all workspaces
var wsResponse = await httpClient.GetAsync("https://api.fabric.microsoft.com/v1/workspaces");
wsResponse.EnsureSuccessStatusCode();
var wsJson = await wsResponse.Content.ReadAsStringAsync();

Console.WriteLine("Workspaces:");
Console.WriteLine(wsJson);

// Parse to get workspace IDs
var wsDoc = JsonDocument.Parse(wsJson);
foreach (var ws in wsDoc.RootElement.GetProperty("value").EnumerateArray())
{
    string workspaceId = ws.GetProperty("id").GetString();
    string workspaceName = ws.GetProperty("displayName").GetString();
    Console.WriteLine($"\n--- Items in workspace: {workspaceName} ---");

    // 2️⃣ List items in each workspace
    var itemsResponse = await httpClient.GetAsync($"https://api.fabric.microsoft.com/v1/workspaces/{workspaceId}/items");
    itemsResponse.EnsureSuccessStatusCode();
    var itemsJson = await itemsResponse.Content.ReadAsStringAsync();
    Console.WriteLine(itemsJson);
}
#endregion
