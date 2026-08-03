using Microsoft.AspNetCore.Components;
using TemplatePWA.PWA.Modules.TestModule.Utiles;
using TemplatePWA.PWA.Shared.Workspace;
using TemplatePWA.PWA.Shared.Common;
using TemplatePWA.Share.Objects.TestModule;
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

		private IEnumerable<Test_Table> FilteredItems =>
			string.IsNullOrWhiteSpace(filtroTexto)
				? Data?.LstTestTable ?? Enumerable.Empty<Test_Table>()
				: (Data?.LstTestTable ?? Enumerable.Empty<Test_Table>())
					.Where(x =>
						(x.vchName?.Contains(filtroTexto, StringComparison.OrdinalIgnoreCase) ?? false)
						|| (x.vchEmail?.Contains(filtroTexto, StringComparison.OrdinalIgnoreCase) ?? false)
						|| x.iIdTest_Table.ToString().Contains(filtroTexto));

		protected override async Task OnInitializedAsync()
		{
			Loading.Show();
			ShowSnake(await Data.PostGetRegistros());
			Loading.Hide();
			_loaded = true;
			SetMenuItems(GetMenuItems());
		}

		private void OpenFormTab(string icon, string text, TipoEstadoControl state, Test_Table registro, bool isNewTab = false)
		{
			var snapshot = CloneRegistro(registro);
			if (state == TipoEstadoControl.Alta)
			{
				snapshot.dtRegistered = DateTime.Now;
			}

			Data.RegistroSeleccionado = snapshot;

			var parameters = new Dictionary<string, object>
			{
				["Data"] = this.Data,
				["EstadoControl"] = state,
				["Registro"] = snapshot
			};

			var tab = new WorkspaceTab
			{
				Icono = icon,
				Text = text,
				TipoControl = typeof(FormTestTable),
				EstadoControl = state,
				EntityId = snapshot.iIdTest_Table,
				ForceNewTab = isNewTab || state == TipoEstadoControl.Alta,
				Parameters = parameters
			};
			AgregarTab(tab);
		}

		private static Test_Table CloneRegistro(Test_Table source)
		{
			if (source == null)
			{
				return new Test_Table();
			}

			return new Test_Table
			{
				iIdTest_Table = source.iIdTest_Table,
				vchName = source.vchName,
				vchEmail = source.vchEmail,
				dtRegistered = source.dtRegistered
			};
		}

		private void OnClickShowForm()
		{
			OpenFormTab(
				Icons.Material.Filled.AddBox,
				"Nuevo Registro",
				TipoEstadoControl.Alta,
				new Test_Table(),
				isNewTab: true);
		}

		private void OnViewRegistro(object registro)
		{
			var entity = (Test_Table)registro;
			OpenFormTab(Icons.Material.Filled.Visibility, $"Ver {entity.iIdTest_Table}", TipoEstadoControl.Lectura, entity);
		}

		private void OnEditRegistro(object registro)
		{
			var entity = (Test_Table)registro;
			OpenFormTab(Icons.Material.Filled.Edit, $"Editar {entity.iIdTest_Table}", TipoEstadoControl.Edicion, entity);
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
				},
				new MenuItemModel
				{
					Text = "Filtrar por TVP",
					Icon = Icons.Material.Filled.FilterList,
					Color = Color.Info,
					Action = EventCallback.Factory.Create(this, OnFilterByDataType)
				}
			};
		}

		/// <summary>
		/// Demo: send current grid IDs (or first two) as DTParameterType list to spGetTestTableDataType.
		/// </summary>
		private async Task OnFilterByDataType()
		{
			Loading.Show();
			var ids = Data.LstTestTable?
				.Take(2)
				.Select(x => x.iIdTest_Table)
				.ToList()
				?? new List<int>();

			ShowSnake(await Data.PostGetByDataType(ids));
			Loading.Hide();
		}
	}
}
