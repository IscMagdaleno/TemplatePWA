# TemplatePWA

A production-ready .NET Blazor WebAssembly PWA template with API integration, built on the **Engrama methodology**. Features a modern tab-based UI with glass-morphism design, MudBlazor components, and a modular architecture for rapid enterprise application development.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with .NET desktop development workload
- SQL Server (LocalDB or full instance)

## Quick Start

1. Clone the repository:
   ```bash
   git clone https://github.com/your-org/TemplatePWA.git
   cd TemplatePWA
   ```

2. Open `TemplatePWA.sln` in Visual Studio 2022.

3. Set the API project as the startup project (right-click → Set as Startup Project).

4. Update the connection string in `TemplatePWA.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "EngramaCloudConnection": "Server=(localdb)\\mssqllocaldb;Database=TemplatePWA;Trusted_Connection=True;"
     }
   }
   ```

5. Run the application:
   ```
   dotnet run --project TemplatePWA.API
   ```

6. Open your browser and navigate to `https://localhost:XXXX`.

## Architecture

```
TemplatePWA/
├── TemplatePWA.API/           # Web API (Controllers, Models, Services)
├── TemplatePWA.PWA/           # Blazor WebAssembly Client
│   ├── Modules/               # Feature modules (CRUD, Forms, Reports)
│   │   └── Module/
│   │       ├── Components/    # Grid and Form components
│   │       └── Utiles/        # Main class (API calls, state)
│   ├── Shared/                # Layout, Workspace, Common
│   │   ├── Workspace/         # Tab system (Service, Tab, Toolbar)
│   │   └── Common/            # Base classes (EngramaPage, EngramaComponent)
│   ├── Pages/                 # Routed pages (non-tab)
│   ├── Helpers/               # Services (LoadingStore, Converters)
│   └── wwwroot/               # Static files, CSS, icons
└── TemplatePWA.Share/         # Shared models (DTOs, Entities)
```

### Key Components

| Component | Purpose |
|-----------|---------|
| `WorkspaceService` | Manages open tabs and menu registration |
| `DynamicComponent` | Renders active tab without module-specific code |
| `ToolbarSuperior` | Dynamic toolbar showing registered modules |
| `MainLayout` | Generic layout with tab header and drawer |

## Adding a New Module

### 1. Create the Model

Add your entity to `TemplatePWA.Share/Objects/ModuleName/`:

```csharp
public class Product
{
    public int iIdProduct { get; set; }
    public string vchName { get; set; } = string.Empty;
    public decimal dPrice { get; set; }
    public bool bActive { get; set; }
}
```

### 2. Create the Main Class

Add to `Modules/ProductModule/Utiles/MainProduct.cs`:

```csharp
public class MainProduct
{
    private readonly string url = "api/Product";
    private readonly IHttpService httpService;
    private readonly IValidaServicioService validaServicioService;

    public IList<Product> LstProduct { get; set; } = new List<Product>();
    public Product Selected { get; set; } = new();
    public TipoEstadoControl EstadoControl { get; set; } = TipoEstadoControl.Lectura;

    public MainProduct(IHttpService httpService, IValidaServicioService validaServicioService)
    {
        this.httpService = httpService;
        this.validaServicioService = validaServicioService;
    }

    public async Task<SeverityMessage> LoadProducts()
    {
        var response = await httpService.Post<object, Response<List<Product>>>(
            $"{url}/GetProducts", new { });
        return validaServicioService.ValidacionServicio(response,
            onSuccess: data => LstProduct = data.ToList());
    }
}
```

### 3. Create Grid Component

Add to `Modules/ProductModule/Components/GridProduct.razor`:

```razor
@inherits EngramaComponent

<MudTable Items="@Data.LstProduct" T="Product" Dense="true" Hover="true" Striped="true">
    <HeaderContent>
        <MudTh>ID</MudTh>
        <MudTh>Name</MudTh>
        <MudTh>Price</MudTh>
        <MudTh>Actions</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd>@context.iIdProduct</MudTd>
        <MudTd>@context.vchName</MudTd>
        <MudTd>$@context.dPrice</MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small"
                OnClick="@(() => SelectProduct(context))" />
        </MudTd>
    </RowTemplate>
</MudTable>
```

### 4. Create Form Component

Add to `Modules/ProductModule/Components/FormProduct.razor`:

```razor
@inherits EngramaComponent

@if (Data.EstadoControl != TipoEstadoControl.Lectura)
{
    <MudCard>
        <MudCardContent>
            <MudTextField @bind-Value="Data.Selected.vchName" Label="Name" />
            <MudNumericField @bind-Value="Data.Selected.dPrice" Label="Price" />
        </MudCardContent>
        <MudCardActions>
            <MudButton OnClick="Save" Variant="Variant.Filled" Color="Color.Primary">
                Save
            </MudButton>
            <MudButton OnClick="Cancel" Variant="Variant.Text">Cancel</MudButton>
        </MudCardActions>
    </MudCard>
}
```

### 5. Register the Module

In your main component's `OnInitializedAsync`:

```csharp
protected override void OnInitializedAsync()
{
    WorkspaceService.AgregarComponenteTab(new WorkspaceTab
    {
        Titulo = "Products",
        Icono = Icons.Material.Filled.Inventory,
        ComponentType = typeof(ProductModule),
        Parameters = new Dictionary<string, object> { { "Data", Data } }
    });
}
```

## Tab System

The template uses a tab-based interface similar to an IDE:

- **CRUD Modules** → Open as tabs (multiple instances allowed)
- **Static Pages** → Use regular routing (`@page "/path"`)

### Tab Lifecycle

1. User clicks menu item → `ToolbarSuperior`
2. `WorkspaceService.AgregarComponenteTab()` creates/activates tab
3. `MainLayout` renders active tab via `DynamicComponent`
4. User can close tabs or switch between them

## Configuration

### API Base URL

Update in `Program.cs`:

```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7196/")
});
```

### Theme Customization

Modify the Aurora theme in `wwwroot/css/app.css`:

```css
:root {
    --aurora-primary: #A50A3A;    /* Maroon */
    --aurora-secondary: #6B1D5E;  /* Purple */
    --aurora-accent: #FF4081;     /* Pink */
}
```

## Project Structure

| Folder | Description |
|--------|-------------|
| `Modules/` | Feature modules with Components and Main classes |
| `Shared/Workspace/` | Tab system infrastructure |
| `Shared/Common/` | Base classes and shared models |
| `Pages/` | Routed pages (home, settings) |
| `Helpers/` | Utility services and converters |
| `wwwroot/` | Static assets, CSS, icons |

## Testing

1. Start the API:
   ```bash
   cd TemplatePWA.API
   dotnet run
   ```

2. Open browser to `https://localhost:XXXX`

3. The home page displays with navigation

4. Click a module in the sidebar to open it as a tab

5. Perform CRUD operations on the test data

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Build fails with file lock errors | Close Visual Studio before building from command line |
| API connection refused | Ensure API is running and URL is correct in Program.cs |
| Tab doesn't open | Verify module is registered in WorkspaceService |
| Infinite loop on page load | Use `OnInitializedAsync` instead of `OnParametersSetAsync` |

## License

MIT License - See [LICENSE](LICENSE) for details.
