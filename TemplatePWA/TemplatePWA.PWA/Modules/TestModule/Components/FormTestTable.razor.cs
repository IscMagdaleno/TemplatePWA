using Microsoft.AspNetCore.Components;
using TemplatePWA.PWA.Modules.TestModule.Utiles;
using TemplatePWA.PWA.Shared.Workspace;
using TemplatePWA.PWA.Shared.Common;
using TemplatePWA.Share.DtoModels.TestModule;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemplatePWA.PWA.Modules.TestModule.Components
{
    public partial class FormTestTable : EngramaComponent
    {
        [Parameter] public MainTest Data { get; set; }
        [Parameter] public EventCallback OnSuccess { get; set; }

        private async Task Submit()
        {
            var model = new DtoaveTest_Table
            {
                iIdTest_Table = Data.RegistroSeleccionado?.iIdTest_Table ?? 0,
                vchName = Data.RegistroSeleccionado?.vchName ?? "",
                vchEmail = Data.RegistroSeleccionado?.vchEmail ?? "",
                dtRegistered = Data.RegistroSeleccionado?.dtRegistered ?? DateTime.Now
            };

            var result = await Data.PostSaveRegistro(model);
            if (result.bResult)
            {
                EstadoControl = TipoEstadoControl.Lectura;
                SetNombreTab("Registro Guardado");
                TriggerMenuUpdate();
                await OnSuccess.InvokeAsync();
            }
        }

        protected override List<MenuItemModel> GetMenuItems()
        {
            var items = new List<MenuItemModel>();

            if (EstadoControl == TipoEstadoControl.Lectura)
            {
                items.Add(new MenuItemModel
                {
                    Text = "Editar",
                    Icon = Icons.Material.Filled.Edit,
                    Color = Color.Primary,
                    Action = EventCallback.Factory.Create(this, () => {
                        EstadoControl = TipoEstadoControl.Edicion;
                        TriggerMenuUpdate();
                    })
                });
            }
            else
            {
                items.Add(new MenuItemModel
                {
                    Text = "Guardar",
                    Icon = Icons.Material.Filled.Save,
                    Color = Color.Success,
                    Action = EventCallback.Factory.Create(this, Submit)
                });
            }

            items.Add(new MenuItemModel
            {
                Text = "Cerrar",
                Icon = Icons.Material.Filled.Close,
                Color = Color.Error,
                Action = EventCallback.Factory.Create(this, CerrarTab)
            });

            return items;
        }
    }
}
