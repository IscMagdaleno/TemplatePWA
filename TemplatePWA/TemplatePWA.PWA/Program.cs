using EngramaCoreStandar.Extensions;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using TemplatePWA.PWA;
using TemplatePWA.PWA.Helpers;
using TemplatePWA.PWA.Shared.Workspace;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var url = "https://localhost:7196/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) });



builder.Services.AddMudServices();

/*Engrama -> Services to call the API using the engrama Tools*/
builder.Services.AddScoped<LoadingState>();

builder.Services.AddEngramaDependenciesBlazor();

/*Workspace Service for tab management*/
builder.Services.AddSingleton<WorkspaceService>();

await builder.Build().RunAsync();
