using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Servicios;

using Microsoft.AspNetCore.Components;

using TemplatePWA.PWA.Areas.TestModul.Utiles;
using TemplatePWA.PWA.Helpers;

namespace TemplatePWA.PWA.Areas.TestModul
{
	public partial class PageTestModule
	{
		[Inject] private IValidaServicioService validaServicioService { get; set; }
		[Inject] private IHttpService httpService { get; set; }
		[Inject] private MapperHelper mapperHelper { get; set; }
		[Inject] private LoadingState Loading { get; set; }

		public DataTest Data { get; set; }

		protected override void OnInitialized()
		{
			Data = new DataTest(httpService, mapperHelper, validaServicioService);
		}

		protected override async Task OnInitializedAsync()
		{
		}



	}
}
