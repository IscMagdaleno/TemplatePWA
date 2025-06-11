using TemplatePWA.PWA.Areas.TestModul.Utiles;
using TemplatePWA.PWA.Shared.Common;

namespace TemplatePWA.PWA.Areas.TestModul
{
	public partial class PageTestModule : EngramaPage
	{


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
