using EngramaCoreStandar.Extensions;

using System.Reflection;

using TemplatePWA.API.EngramaLevels.Dominio.Core;
using TemplatePWA.API.EngramaLevels.Dominio.Interfaces;
using TemplatePWA.API.EngramaLevels.Infrastructure.Interfaces;
using TemplatePWA.API.EngramaLevels.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<ITestDominio, TestDominio>();
builder.Services.AddScoped<ITestRepository, TestRepository>();


// Ensure the AddEngramaDependenciesAPI method is defined in the above namespace
builder.Services.AddEngramaDependenciesAPI();



/*Swagger configuration*/
builder.Services.AddSwaggerGen(options =>
{
	// using System.Reflection;
	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var path = Path.Combine(AppContext.BaseDirectory, xmlFilename);
	options.IncludeXmlComments(path);

});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(x => x
					.AllowAnyMethod()
					.AllowAnyHeader()
					.SetIsOriginAllowed(origin => true) // allow any origin
					.AllowCredentials());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
