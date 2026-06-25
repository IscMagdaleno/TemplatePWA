using Microsoft.AspNetCore.Components;
using TemplatePWA.PWA.Shared.Common;
using System.Collections.Generic;

namespace TemplatePWA.PWA.Shared.Workspace
{
    public class EngramaWorkspaceComponent : ComponentBase
    {
        [Inject] protected WorkspaceService WorkspaceService { get; set; } = default!;

        private TipoEstadoControl _estadoControl = TipoEstadoControl.Lectura;

        public TipoEstadoControl EstadoControl
        {
            get => _estadoControl;
            set
            {
                if (_estadoControl != value)
                {
                    _estadoControl = value;
                    WorkspaceService.SetCurrentTabState(_estadoControl);
                }
            }
        }

        protected void AgregarTab(WorkspaceTab tab)
        {
            WorkspaceService.AddComponenteTab(tab);
        }

        protected void CerrarTab()
        {
            WorkspaceService.SetCloseCurrentTab(EstadoControl == TipoEstadoControl.Alta || EstadoControl == TipoEstadoControl.Edicion);
        }

        protected void SetMenuItems(List<MenuItemModel> menuItems)
        {
            WorkspaceService.SetMenuItems(menuItems);
        }

        protected virtual List<MenuItemModel> GetMenuItems()
        {
            return new List<MenuItemModel>();
        }
    }
}
