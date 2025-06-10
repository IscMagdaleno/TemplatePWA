using EngramaCoreStandar.Dapper.Results;
using EngramaCoreStandar.Extensions;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using TemplatePWA.PWA.Areas.TestModul.Utiles;
using TemplatePWA.PWA.Helpers;

namespace TemplatePWA.PWA.Areas.TestModul.Components
{
	public partial class TableTestTable
	{
		[Inject] private LoadingState Loading { get; set; }
		[Inject] private ISnackbar Snackbar { get; set; }
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



		public void ShowSnake(SeverityMessage severityMessage)
		{
			if (severityMessage.vchMessage.NotEmpty())
			{

				Snackbar.Clear();
				Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
				Snackbar.Add(severityMessage.vchMessage, MudBlazorConverter.ConvertSeverity(severityMessage.Severity));
			}
		}

	}
}
