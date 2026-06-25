using Microsoft.AspNetCore.Components;
using TemplatePWA.PWA.Modules.TestModule.Utiles;
using TemplatePWA.PWA.Shared.Workspace;
using TemplatePWA.PWA.Shared.Common;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemplatePWA.PWA.Modules.TestModule.Components
{
    public partial class GridTestTable : EngramaComponent
    {
        [Parameter] public MainTest Data { get; set; }
        
        public string filtroTexto { get; set; } = "";
        private bool _loaded = false;

        protected override async Task OnInitializedAsync()
        {
            Loading.Show();
            ShowSnake(await Data.PostGetRegistros());
            Loading.Hide();
            _loaded = true;
            SetMenuItems(GetMenuItems());
        }

        private void OpenFormTab(string icon, string text, TipoEstadoControl state)
        {
            Data.RegistroSeleccionado = new TemplatePWA.Share.Objects.TestModule.Test_Table();
            var tab = new WorkspaceTab
            {
                Icono = icon,
                Text = text,
                TipoControl = typeof(FormTestTable),
                Parameters = new Dictionary<string, object>
                {
                    ["Data"] = this.Data,
                    ["EstadoControl"] = state
                }
            };
            AgregarTab(tab);
        }

        private void OnClickShowForm()
        {
            OpenFormTab(Icons.Material.Filled.AddBox, "Nuevo Registro", TipoEstadoControl.Alta);
        }

        private void OnViewRegistro(object registro)
        {
            Data.RegistroSeleccionado = (TemplatePWA.Share.Objects.TestModule.Test_Table)registro;
            OpenFormTab(Icons.Material.Filled.Visibility, "Ver Registro", TipoEstadoControl.Lectura);
        }

        private void OnEditRegistro(object registro)
        {
            Data.RegistroSeleccionado = (TemplatePWA.Share.Objects.TestModule.Test_Table)registro;
            OpenFormTab(Icons.Material.Filled.Edit, "Editar Registro", TipoEstadoControl.Edicion);
        }

        protected override List<MenuItemModel> GetMenuItems()
        {
            return new List<MenuItemModel>
            {
                new MenuItemModel
                {
                    Text = "Agregar",
                    Icon = Icons.Material.Filled.Add,
                    Color = Color.Success,
                    Action = EventCallback.Factory.Create(this, OnClickShowForm)
                }
            };
        }
    }
}
