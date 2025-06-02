using EngramaCoreStandar.Logger;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Servicios;

using Microsoft.AspNetCore.Components;

using System.Reflection;

using TemplatePWA.PWA.Areas.TestModul.Utiles;
using TemplatePWA.PWA.Helpers;

namespace TemplatePWA.PWA.Areas.TestModul
{
	public partial class PageTestModule
	{
		[Inject] private IValidaServicioService validaServicioService { get; set; }

		[Inject] private IHttpService httpService { get; set; }
		[Inject] private MapperHelper mapperHelper { get; set; }
		[Inject] private ILoggerHelper _LoggerHelper { get; set; }
		private string GetPath() => GetType().ToString() + " - ";
		[Inject] private LoadingState Loading { get; set; }

		public DataTest Data { get; set; }

		protected override void OnInitialized()
		{
			_LoggerHelper.Info($"{GetPath()}{MethodBase.GetCurrentMethod().Name}");
			Data = new DataTest(httpService, mapperHelper, validaServicioService);
		}

		protected override async Task OnInitializedAsync()
		{
			_LoggerHelper.Info($"{GetPath()}{MethodBase.GetCurrentMethod().Name}");
		}



	}
}
