using Microsoft.AspNetCore.Components;
using TemplatePWA.PWA.Modules.TestModule.Utiles;
using TemplatePWA.PWA.Shared.Workspace;
using TemplatePWA.PWA.Shared.Common;
using TemplatePWA.Share.DtoModels.TestModule;
using TemplatePWA.Share.Objects.TestModule;
using EngramaCoreStandar.Dapper.Results;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TemplatePWA.PWA.Modules.TestModule.Components
{
	public partial class FormTestTable : EngramaComponent
	{
		[Parameter] public MainTest Data { get; set; }
		[Parameter] public new TipoEstadoControl EstadoControl { get; set; }
		[Parameter] public Test_Table Registro { get; set; }

		protected override void OnInitialized()
		{
			if (Registro == null)
			{
				Registro = new Test_Table();
			}

			if (EstadoControl == TipoEstadoControl.Alta
				&& (Registro.dtRegistered == default || Registro.dtRegistered < new DateTime(1900, 1, 2)))
			{
				Registro.dtRegistered = DateTime.Now;
			}

			Data.RegistroSeleccionado = Registro;
			EstadoControl = EstadoControl;
			SetMenuItems(GetMenuItems());
		}

		private void EnableEdit()
		{
			EstadoControl = TipoEstadoControl.Edicion;
			WorkspaceService.SetMenuItems(GetMenuItems());
			StateHasChanged();
		}

		private async Task Submit()
		{
			if (string.IsNullOrWhiteSpace(Registro?.vchName) || string.IsNullOrWhiteSpace(Registro?.vchEmail))
			{
				ShowSnake(new SeverityMessage(false, "Nombre y Email son obligatorios.", SeverityTag.Warning));
				return;
			}

			Loading.Show();
			var model = new DtoaveTest_Table
			{
				iIdTest_Table = Registro?.iIdTest_Table ?? 0,
				vchName = Registro?.vchName ?? "",
				vchEmail = Registro?.vchEmail ?? "",
				dtRegistered = Registro?.dtRegistered ?? DateTime.Now
			};

			var result = await Data.PostSaveRegistro(model);
			ShowSnake(result);
			Loading.Hide();

			if (result.bResult)
			{
				Registro.iIdTest_Table = Data.RegistroSeleccionado?.iIdTest_Table ?? Registro.iIdTest_Table;
				EstadoControl = TipoEstadoControl.Lectura;
				WorkspaceService.SetCurrentTabName(
					Registro.iIdTest_Table > 0 ? $"Registro {Registro.iIdTest_Table}" : "Registro Guardado",
					Icons.Material.Filled.Check);
				WorkspaceService.SetMenuItems(GetMenuItems());
				StateHasChanged();
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
					Action = EventCallback.Factory.Create(this, EnableEdit)
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
