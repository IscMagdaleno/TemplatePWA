using TemplatePWA.PWA.Modules.TestModule.Utiles;
using TemplatePWA.PWA.Shared.Common;

namespace TemplatePWA.PWA.Modules.TestModule
{
	public partial class PageTest : EngramaPage
	{
		public MainTest Data { get; set; }

		protected override void OnInitialized()
		{
			Data = new MainTest(httpService, mapperHelper, validaServicioService);
		}

		protected override async Task OnInitializedAsync()
		{

		}
	}
}
