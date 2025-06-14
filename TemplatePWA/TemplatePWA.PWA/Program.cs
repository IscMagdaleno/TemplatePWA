using EngramaCoreStandar.Extensions;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;

using TemplatePWA.PWA;
using TemplatePWA.PWA.Helpers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


var url = "https://localhost:7196/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(url) });



builder.Services.AddMudServices();

/*Engrama -> Services to call the API using the engrama Tools*/
builder.Services.AddScoped<LoadingState>();

builder.Services.AddEngramaDependenciesBlazor();

await builder.Build().RunAsync();
