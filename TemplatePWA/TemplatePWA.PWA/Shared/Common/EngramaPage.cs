using EngramaCoreStandar.Dapper.Results;
using EngramaCoreStandar.Extensions;
using EngramaCoreStandar.Mapper;
using EngramaCoreStandar.Servicios;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using TemplatePWA.PWA.Helpers;
using TemplatePWA.PWA.Shared.Workspace;

namespace TemplatePWA.PWA.Shared.Common
{
    public class EngramaPage : EngramaWorkspaceComponent
    {
        [Inject] protected IValidaServicioService validaServicioService { get; set; }
        [Inject] protected IHttpService httpService { get; set; }
        [Inject] protected MapperHelper mapperHelper { get; set; }
        [Inject] protected new LoadingState Loading { get; set; }

        [Inject] protected ISnackbar Snackbar { get; set; }


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
