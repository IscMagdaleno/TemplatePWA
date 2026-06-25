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

        protected override async Task OnInitializedAsync()
        {
            Loading.Show();
            ShowSnake(await Data.PostGetRegistros());
            Loading.Hide();
        }

        private void OnClickShowForm()
        {
            Data.RegistroSeleccionado = new TemplatePWA.Share.Objects.TestModule.Test_Table();
            var type = typeof(FormTestTable);
            var tab = new WorkspaceTab
            {
                Icono = Icons.Material.Filled.AddBox,
                Text = "Nuevo Registro",
                TipoControl = type,
                Repetir = false,
                EstadoControl = TipoEstadoControl.Alta
            };

            tab.Componente = builder =>
            {
                builder.OpenComponent(0, type);
                builder.AddAttribute(1, "Data", this.Data);
                builder.AddComponentReferenceCapture(2, instance =>
                {
                    if (instance is EngramaWorkspaceComponent baseComponent)
                    {
                        tab.InstanciaComponente = baseComponent;
                        baseComponent.IconoBase = tab.Icono ?? "";
                        baseComponent.EstadoControl = tab.EstadoControl;
                        baseComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            AgregarTab(tab);
        }

        private void OnViewRegistro(object registro)
        {
            Data.RegistroSeleccionado = (TemplatePWA.Share.Objects.TestModule.Test_Table)registro;
            var type = typeof(FormTestTable);
            var tab = new WorkspaceTab
            {
                Icono = Icons.Material.Filled.Visibility,
                Text = "Ver Registro",
                TipoControl = type,
                Repetir = true,
                EstadoControl = TipoEstadoControl.Lectura
            };

            tab.Componente = builder =>
            {
                builder.OpenComponent(0, type);
                builder.AddAttribute(1, "Data", this.Data);
                builder.AddComponentReferenceCapture(2, instance =>
                {
                    if (instance is EngramaWorkspaceComponent baseComponent)
                    {
                        tab.InstanciaComponente = baseComponent;
                        baseComponent.IconoBase = tab.Icono ?? "";
                        baseComponent.EstadoControl = tab.EstadoControl;
                        baseComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            AgregarTab(tab);
        }

        private void OnEditRegistro(object registro)
        {
            Data.RegistroSeleccionado = (TemplatePWA.Share.Objects.TestModule.Test_Table)registro;
            var type = typeof(FormTestTable);
            var tab = new WorkspaceTab
            {
                Icono = Icons.Material.Filled.Edit,
                Text = "Editar Registro",
                TipoControl = type,
                Repetir = true,
                EstadoControl = TipoEstadoControl.Edicion
            };

            tab.Componente = builder =>
            {
                builder.OpenComponent(0, type);
                builder.AddAttribute(1, "Data", this.Data);
                builder.AddComponentReferenceCapture(2, instance =>
                {
                    if (instance is EngramaWorkspaceComponent baseComponent)
                    {
                        tab.InstanciaComponente = baseComponent;
                        baseComponent.IconoBase = tab.Icono ?? "";
                        baseComponent.EstadoControl = tab.EstadoControl;
                        baseComponent.TriggerMenuUpdate();
                    }
                });
                builder.CloseComponent();
            };

            AgregarTab(tab);
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
