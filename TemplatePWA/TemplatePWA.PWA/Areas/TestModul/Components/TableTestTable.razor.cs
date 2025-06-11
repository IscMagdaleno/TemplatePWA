using Microsoft.AspNetCore.Components;

using TemplatePWA.PWA.Areas.TestModul.Utiles;
using TemplatePWA.PWA.Shared.Common;

namespace TemplatePWA.PWA.Areas.TestModul.Components
{
	public partial class TableTestTable : EngramaComponent
	{

		[Parameter] public DataTest Data { get; set; }

		protected override void OnInitialized()
		{

		}

		protected override async Task OnInitializedAsync()
		{
			Loading.Show();
			ShowSnake(await Data.PostTestTable());
			Loading.Hide();
		}





	}
}
