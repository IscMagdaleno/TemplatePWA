using Microsoft.AspNetCore.Components;
using TemplatePWA.PWA.Shared.Common;
using System.Collections.Generic;

namespace TemplatePWA.PWA.Shared.Workspace
{
    public class EngramaWorkspaceComponent : ComponentBase
    {
        [Inject] protected WorkspaceService WorkspaceService { get; set; } = default!;

        public string IconoBase = "Icons.Material.Filled.Info";

        private TipoEstadoControl _estadoControl = TipoEstadoControl.Lectura;

        public TipoEstadoControl EstadoControl
        {
            get => _estadoControl;
            set
            {
                if (_estadoControl != value)
                {
                    _estadoControl = value;
                    OnEstadoControlChanged();
                }
            }
        }

        protected void OnEstadoControlChanged()
        {
            WorkspaceService.SetCurrentTabState(this._estadoControl);
        }

        protected void AgregarTab(WorkspaceTab tab)
        {
            WorkspaceService.AddComponenteTab(tab);
        }

        protected void SetNombreTab(string tabName)
        {
            string iconText = EstadoControl == TipoEstadoControl.Alta
                ? "Icons.Material.Filled.Add"
                : EstadoControl == TipoEstadoControl.Edicion
                    ? "Icons.Material.Filled.Edit"
                    : IconoBase;

            WorkspaceService.SetCurrentTabName(tabName, iconText);
        }

        protected void CerrarTab()
        {
            WorkspaceService.SetCloseCurrentTab(this.EstadoControl == TipoEstadoControl.Alta || this.EstadoControl == TipoEstadoControl.Edicion);
        }

        protected void SetMenuItems(List<MenuItemModel> menuItems)
        {
            WorkspaceService.SetMenuItems(menuItems);
        }

        public void TriggerMenuUpdate()
        {
            WorkspaceService.SetMenuItems(GetMenuItems());
        }

        protected virtual List<MenuItemModel> GetMenuItems()
        {
            return new List<MenuItemModel>();
        }
    }
}
