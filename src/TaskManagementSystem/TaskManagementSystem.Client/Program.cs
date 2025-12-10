using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskManagementSystem.Client;
using TaskManagementSystem.Client.AuthenticationProvider;
using TaskManagementSystem.Client.Handlers.Authentication;
using TaskManagementSystem.Client.Helper;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Services
builder.Services.AddBlazoredLocalStorage(); //Register the Blazored Storage
builder.Services.AddTransient<AuthStateHandler>(); //Registers the authstatehandler as a transient service
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<RefreshTokenHandler>();
builder.Services.AddScoped<AuthenticationSignoutHandler>();
builder.Services.AddScoped<AuthenticationSignInHandler>();
builder.Services.AddScoped<ChangePasswordHandler>();

//Clients
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//Open Client Named Registration
builder.Services.AddHttpClient(ClientHelper.OpenClientKey, opts =>
{
    opts.BaseAddress = new Uri(ClientHelper.BaseUri);
});

//Secure Client Named Registration by adding the authstatehandler for adding session token.
builder.Services.AddHttpClient(ClientHelper.SecureClientKey, opts =>
{
    opts.BaseAddress = new Uri(ClientHelper.BaseUri);
    opts.Timeout = TimeSpan.FromMilliseconds(10000);

}).AddHttpMessageHandler<AuthStateHandler>();

await builder.Build().RunAsync();
