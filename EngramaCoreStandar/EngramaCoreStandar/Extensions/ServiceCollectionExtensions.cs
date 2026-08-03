using EngramaCoreStandar.Dapper;
using EngramaCoreStandar.Logger;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Results;
using EngramaCoreStandar.Servicios;

using Microsoft.Extensions.DependencyInjection;

namespace EngramaCoreStandar.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddEngramaDependenciesAPI(this IServiceCollection services)
		{
			services.AddScoped<IResponseHelper, ResponseHelper>();
			services.AddScoped<IDapperManager, DapperManager>();
			services.AddScoped<IDapperManagerHelper, DapperMangerHelper>();
			services.AddScoped<ILoggerHelper, LoggerHelper>();
			services.AddSingleton<MapperHelper>();

			return services;
		}

		public static IServiceCollection AddEngramaDependenciesBlazor(this IServiceCollection services)
		{
			services.AddScoped<IValidaServicioService, ValidaServicioService>();
			services.AddScoped<IHttpService, HttpService>();
			services.AddScoped<ILoggerHelper, LoggerHelper>();
			services.AddSingleton<MapperHelper>(); // MapperHelper como singleton porque maneja su propia configuración



			return services;
		}
	}
}
