using Microsoft.AspNetCore.Components;

using TemplatePWA.PWA.Modules.TestModule.Utiles;
using TemplatePWA.PWA.Shared.Common;

namespace TemplatePWA.PWA.Modules.TestModule.Components
{
	public partial class TableTestTable : EngramaComponent
	{

		[Parameter] public MainTest Data { get; set; }

		protected override async Task OnInitializedAsync()
		{
			Loading.Show();
			ShowSnake(await Data.PostTestTable());
			Loading.Hide();
		}

	}
}
