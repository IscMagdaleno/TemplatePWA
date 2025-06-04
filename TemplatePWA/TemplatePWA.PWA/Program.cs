using EngramaCoreStandar.Logger;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Servicios;

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
builder.Services.AddScoped<IValidaServicioService, ValidaServicioService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ILoggerHelper, LoggerHelper>();
builder.Services.AddSingleton<MapperHelper>(); // MapperHelper como singleton porque maneja su propia configuración


await builder.Build().RunAsync();
