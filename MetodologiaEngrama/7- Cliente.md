# Metodología Engrama: Capa de Cliente (Blazor WebAssembly PWA)

La capa de **Cliente** en la metodología Engrama utiliza **.NET Blazor WebAssembly** en su versión más reciente como tecnología principal para desarrollar una interfaz gráfica moderna, responsiva y funcional, configurada como una **Progressive Web App (PWA)**. Esta capa permite a los usuarios interactuar con la aplicación a través de componentes visuales, aprovechando la biblioteca **MudBlazor** para simplificar el desarrollo de componentes funcionales y estilizados. Este documento detalla la estructura, convenciones y ejemplos para el cliente, asegurando homogeneidad con las capas de Servicio, Dominio e Infraestructura.

## Tecnologías y Herramientas

- **.NET Blazor WebAssembly**: Framework utilizado para crear aplicaciones web interactivas, optimizadas para ejecución en el navegador.
- **MudBlazor**: Biblioteca de componentes que proporciona elementos de interfaz predefinidos, como tablas, formularios y barras de notificación, para acelerar el desarrollo.
- **EngramaCoreStandard**: Paquete que incluye servicios y clases estándar de Engrama, como `IHttpService`, `MapperHelper`, y `IValidaServicioService`, para estandarizar la interacción con la API y la lógica del cliente.

## Estructura del Cliente

La estructura de carpetas en el proyecto cliente está diseñada para mantener la modularidad y alinearse con los módulos definidos en las capas inferiores. Las carpetas principales son:

1. **wwwroot**:
   - Contiene archivos estáticos como CSS, JavaScript e imágenes necesarios para el diseño y funcionamiento de la aplicación.
   - Ejemplo: Archivos de estilos personalizados o scripts de terceros.

2. **Modules**:
   - Organiza la aplicación en submódulos, cada uno representando una sección funcional (por ejemplo, `Usuarios`, `Productos`).
   - Cada submódulo contiene:
     - **Components**: Archivos `.razor` que contienen componentes reutilizables administrados por las páginas.
     - **Main Class**: Una clase con prefijo `Main` (por ejemplo, `MainUsuarios`) que centraliza la lógica del módulo, incluyendo llamadas a la API y manejo de datos.

3. **Shared**:
   - Contiene archivos compartidos como layouts, modelos de menú y servicios del workspace.
   - Incluye `MainLayout.razor`, `WorkspaceService.cs`, `WorkspaceTab.cs`, y `ToolbarSuperior.razor`.

4. **Pages**:
   - Contiene páginas de navegación directa (sin tabs) como la página de inicio.

5. **Helpers**:
   - Contiene servicios auxiliares como `LoadingStore.cs` y conversores como `MudBlazorConverter.cs`.

## Arquitectura de Tabs (Workspace System)

La arquitectura de tabs permite abrir múltiples módulos simultáneamente en pestañas, similar a un IDE. El sistema se compone de:

### Flujo del Sistema de Tabs

```
Usuario hace clic en menú → ToolbarSuperior → WorkspaceService.AgregarComponenteTab()
    → Evento MenuChanged → MainLayout.AbrirComponenteEnPestaña()
    → DynamicComponent renderiza el componente activo
```

### WorkspaceService (Servicio de Tabs)

El `WorkspaceService` es el中枢 central que gestiona el estado de las pestañas y los eventos del menú:

```csharp
public class WorkspaceService
{
    // Lista de pestañas abiertas
    public List<WorkspaceTab> LstWorkspaceTabs { get; set; } = new();

    // Pestaña actualmente activa
    public WorkspaceTab? TabActiva { get; set; }

    // Evento que se dispara cuando se agrega/quita una pestaña
    public event Action? OnTabsChanged;

    // Evento que se dispara cuando el menú solicita abrir un componente
    public event Action<WorkspaceTab>? MenuChanged;

    /// <summary>
    /// Registra un componente como opción de menú en el toolbar.
    /// Se llama desde OnInitializedAsync de cada módulo.
    /// </summary>
    public void AddComponenteTab(string module, string texto, string icon, 
        Type componentType, Dictionary<string, object> parameters = null)
    {
        if (!LstMenuItem.Any(x => x.Text == texto))
        {
            LstMenuItem.Add(new MenuItemModel
            {
                Module = module,
                Text = texto,
                Icon = icon,
                ComponentType = componentType,
                Parameters = parameters
            });
            OnMenuChanged();
        }
    }

    /// <summary>
    /// Abre un componente en una nueva pestaña o activa existente.
    /// </summary>
    public void AgregarComponenteTab(WorkspaceTab tab)
    {
        var existing = LstWorkspaceTabs.FirstOrDefault(x => 
            x.ComponentType == tab.ComponentType);

        if (existing != null)
        {
            TabActiva = existing;
        }
        else
        {
            tab.Id = Guid.NewGuid().ToString();
            LstWorkspaceTabs.Add(tab);
            TabActiva = tab;
        }

        OnTabsChanged?.Invoke();
        MenuChanged?.Invoke(tab);
    }

    /// <summary>
    /// Cierra una pestaña y selecciona la adyacente.
    /// </summary>
    public void CerrarPestaña(string tabId)
    {
        var tab = LstWorkspaceTabs.FirstOrDefault(x => x.Id == tabId);
        if (tab == null) return;

        var index = LstWorkspaceTabs.IndexOf(tab);
        LstWorkspaceTabs.Remove(tab);

        if (TabActiva?.Id == tabId)
        {
            TabActiva = LstWorkspaceTabs.Count > 0
                ? LstWorkspaceTabs[Math.Min(index, LstWorkspaceTabs.Count - 1)]
                : null;
        }

        OnTabsChanged?.Invoke();
    }

    /// <summary>
    /// Selecciona una pestaña como activa.
    /// </summary>
    public void SeleccionarPestaña(string tabId)
    {
        TabActiva = LstWorkspaceTabs.FirstOrDefault(x => x.Id == tabId);
        OnTabsChanged?.Invoke();
    }
}
```

### WorkspaceTab (Modelo de Tab)

Cada pestaña se representa con un `WorkspaceTab` que contiene el tipo de componente a renderizar y sus parámetros:

```csharp
public class WorkspaceTab
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Titulo { get; set; } = string.Empty;
    public string Icono { get; set; } = Icons.Material.Filled.Tab;
    public Type ComponentType { get; set; } = default!;
    public Dictionary<string, object> Parameters { get; set; } = new();
}
```

### MainLayout (Layout Genérico con DynamicComponent)

El `MainLayout` es genérico y no contiene código específico de módulos. Utiliza `DynamicComponent` para renderizar el componente activo:

```razor
@inherits LayoutComponentBase

@using Microsoft.AspNetCore.Components
@using Microsoft.AspNetCore.Components.Web
@using MudBlazor
@using TemplatePWA.PWA.Shared.Workspace

<div class="aurora-background">
    <div class="aurora-layer aurora-1"></div>
    <div class="aurora-layer aurura-2"></div>
    <div class="aurora-layer aurora-3"></div>
</div>

<MudThemeProvider Theme="_theme" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudLayout>
    <MudAppBar Elevation="1" Class="glassmorphism-appbar">
        <MudIconButton Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" 
            OnClick="ToggleDrawer" />
        <MudText Typo="Typo.h6" Class="px-4" Style="color: #FFFFFF;">
            Template PWA
        </MudText>
        <MudSpacer />
        <ToolbarSuperior />
    </MudAppBar>

    <MudDrawer @bind-Open="_drawerOpen" ClipMode="DrawerClipMode.Always" 
        Elevation="2" Class="glassmorphism-drawer">
        <MudNavMenu>
            <MudNavLink Href="/" Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Home">
                Inicio
            </MudNavLink>
            @foreach (var module in _workspaceService.LstMenuItem)
            {
                <MudNavLink Icon="@module.Icon" 
                    OnClick="@(() => _workspaceService.AgregarComponenteTab(new WorkspaceTab
                    {
                        Titulo = module.Text,
                        Icono = module.Icon,
                        ComponentType = module.ComponentType,
                        Parameters = module.Parameters ?? new()
                    }))">
                    @module.Text
                </MudNavLink>
            }
        </MudNavMenu>
    </MudDrawer>

    <MudMainContent Class="glassmorphism-main">
        <div class="main-content-wrapper">
            @if (_workspaceService.TabActiva != null)
            {
                <div class="tabs-header">
                    @foreach (var tab in _workspaceService.LstWorkspaceTabs)
                    {
                        <div class="tab-item @(tab.Id == _workspaceService.TabActiva?.Id ? "active" : "")"
                            @onclick="@(() => _workspaceService.SeleccionarPestaña(tab.Id))">
                            <MudIcon Icon="@tab.Icono" Size="Size.Small" />
                            <span>@tab.Titulo</span>
                            <MudIconButton Icon="@Icons.Material.Filled.Close" Size="Size.Small"
                                OnClick="@(() => _workspaceService.CerrarPestaña(tab.Id))" />
                        </div>
                    }
                </div>

                <div class="tab-content">
                    <DynamicComponent Type="@_workspaceService.TabActiva.ComponentType" 
                        Parameters="@_workspaceService.TabActiva.Parameters" />
                </div>
            }
            else
            {
                @Body
            }
        </div>
    </MudMainContent>
</MudLayout>
```

#### Explicación
- **DynamicComponent**: Renderiza dinámicamente el componente activo basado en `TabActiva.ComponentType`
- **Parameters**: Pasa parámetros al componente usando `Dictionary<string, object>`
- **Genérico**: No hay referencias a módulos específicos; todo se registra dinámicamente

### ToolbarSuperior (Barra de Herramientas Dinámica)

El `ToolbarSuperior` muestra los módulos registrados como opciones de menú:

```razor
@using TemplatePWA.PWA.Shared.Workspace
@inject WorkspaceService WorkspaceService

<div class="toolbar-superior">
    @foreach (var menu in WorkspaceService.LstMenuItem)
    {
        <MudTooltip Text="@menu.Text">
            <MudIconButton Icon="@menu.Icon" Color="Color.Inherit"
                OnClick="@(() => WorkspaceService.AgregarComponenteTab(new WorkspaceTab
                {
                    Titulo = menu.Text,
                    Icono = menu.Icon,
                    ComponentType = menu.ComponentType,
                    Parameters = menu.Parameters ?? new()
                }))" />
        </MudTooltip>
    }
</div>
```

## Convenciones para Componentes CRUD (Tabs)

Los módulos CRUD se renderizan como tabs usando el sistema workspace. Cada módulo tiene:

### Estructura del Módulo

```
Modules/
└── MiModulo/
    ├── Components/
    │   ├── GridMiEntidad.razor
    │   ├── GridMiEntidad.razor.cs
    │   ├── FormMiEntidad.razor
    │   └── FormMiEntidad.razor.cs
    └── Utiles/
        └── MainMiEntidad.cs
```

### Main Class (Centralización de Lógica)

La clase `Main` centraliza la lógica del módulo, incluyendo llamadas a la API:

```csharp
public class MainMiEntidad
{
    private readonly string url = "api/MiEntidad";
    private readonly IHttpService httpService;
    private readonly MapperHelper mapperHelper;
    private readonly IValidaServicioService validaServicioService;

    public IList<MiEntidad> LstMiEntidad { get; set; } = new List<MiEntidad>();
    public MiEntidad EntidadSeleccionada { get; set; } = new();
    public TipoEstadoControl EstadoControl { get; set; } = TipoEstadoControl.Lectura;

    public MainMiEntidad(IHttpService httpService, MapperHelper mapperHelper, 
        IValidaServicioService validaServicioService)
    {
        this.httpService = httpService;
        this.mapperHelper = mapperHelper;
        this.validaServicioService = validaServicioService;
    }

    public async Task<SeverityMessage> PostGetRegistros()
    {
        var apiUrl = $"{url}/PostGetRegistros";
        var model = new DtoMiEntidad();

        var response = await httpService.Post<DtoMiEntidad, Response<List<MiEntidad>>>(apiUrl, model);
        var validation = validaServicioService.ValidacionServicio(response,
            onSuccess: data => LstMiEntidad = data.ToList());

        return validation;
    }

    public async Task<SeverityMessage> PostSaveRegistro()
    {
        var apiUrl = $"{url}/PostSaveRegistro";
        var response = await httpService.Post<MiEntidad, Response<bool>>(apiUrl, EntidadSeleccionada);
        var validation = validaServicioService.ValidacionServicio(response,
            onSuccess: data => { /* éxito */ });

        return validation;
    }
}
```

### Componente Grid (Tabla de Registros)

El componente Grid muestra los datos y permite seleccionar registros:

```razor
@inherits EngramaComponent

<MudTable Items="@Data.LstMiEntidad" T="MiEntidad" Dense="true" Hover="true"
    RowsPerPage="15" Height="600px" Striped="true">
    <HeaderContent>
        <MudTh>ID</MudTh>
        <MudTh>Nombre</MudTh>
        <MudTh>Acciones</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd DataLabel="ID">@context.iId</MudTd>
        <MudTd DataLabel="Nombre">@context.vchNombre</MudTd>
        <MudTd>
            <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small"
                OnClick="@(() => SeleccionarRegistro(context))" />
        </MudTd>
    </RowTemplate>
</MudTable>
```

```csharp
public partial class GridMiEntidad : EngramaComponent
{
    [Parameter] public MainMiEntidad Data { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Loading.Show();
        var result = await Data.PostGetRegistros();
        ShowSnake(result);
        Loading.Hide();
    }

    private void SeleccionarRegistro(MiEntidad registro)
    {
        Data.EntidadSeleccionada = registro;
        Data.EstadoControl = TipoEstadoControl.Edicion;
        StateHasChanged();
    }
}
```

### Componente Form (Formulario de Edición)

El componente Form maneja la edición y creación de registros:

```razor
@inherits EngramaComponent

@if (Data.EstadoControl != TipoEstadoControl.Lectura)
{
    <MudCard>
        <MudCardContent>
            <MudTextField @bind-Value="Data.EntidadSeleccionada.vchNombre" 
                Label="Nombre" />
        </MudCardContent>
        <MudCardActions>
            <MudButton OnClick="@Guardar" Variant="Variant.Filled" 
                Color="Color.Primary">Guardar</MudButton>
            <MudButton OnClick="@Cancelar" Variant="Variant.Text">Cancelar</MudButton>
        </MudCardActions>
    </MudCard>
}
```

```csharp
public partial class FormMiEntidad : EngramaComponent
{
    [Parameter] public MainMiEntidad Data { get; set; }

    private async Task Guardar()
    {
        Loading.Show();
        var result = await Data.PostSaveRegistro();
        ShowSnake(result);
        Loading.Hide();

        if (result.bResult)
        {
            Data.EstadoControl = TipoEstadoControl.Lectura;
            Data.EntidadSeleccionada = new();
            StateHasChanged();
        }
    }

    private void Cancelar()
    {
        Data.EstadoControl = TipoEstadoControl.Lectura;
        Data.EntidadSeleccionada = new();
        StateHasChanged();
    }
}
```

### Registro del Módulo (OnInitializedAsync)

Cada módulo se registra en el `WorkspaceService` desde `OnInitializedAsync` de su componente principal:

```csharp
public partial class MainMiEntidad : EngramaComponent
{
    [Inject] private WorkspaceService WorkspaceService { get; set; }
    [Inject] private IHttpService httpService { get; set; }
    [Inject] private MapperHelper mapperHelper { get; set; }
    [Inject] private IValidaServicioService validaServicioService { get; set; }

    public MainMiEntidad Data { get; set; }

    protected override void OnInitialized()
    {
        Data = new MainMiEntidad(httpService, mapperHelper, validaServicioService);

        // Registrar este módulo en el workspace
        WorkspaceService.AgregarComponenteTab(new WorkspaceTab
        {
            Titulo = "Mi Entidad",
            Icono = Icons.Material.Filled.TableChart,
            ComponentType = typeof(MainMiEntidad),
            Parameters = new Dictionary<string, object> { { "Data", Data } }
        });
    }
}
```

## Páginas de Navegación Directa (Sin Tabs)

Las páginas que no requieren tabs (como la página de inicio) usan navegación regular con `@page`:

### Ejemplo de Página con Ruta

```razor
@page "/"
@inherits EngramaPage

<MudContainer MaxWidth="MaxWidth.Large" Class="mt-4">
    <MudText Typo="Typo.h4" Class="mb-4">Inicio</MudText>
    <MudCard>
        <MudCardContent>
            <MudText>Bienvenido al sistema</MudText>
        </MudCardContent>
    </MudCard>
</MudContainer>
```

#### Explicación
- Usa `@page "/"` para navegación directa
- Hereda de `EngramaPage` para acceder a servicios
- No se registra en el `WorkspaceService`

## Clases Base

### EngramaWorkspaceComponent (Base del Workspace)

Todos los componentes del workspace heredan de `EngramaWorkspaceComponent`:

```csharp
public class EngramaWorkspaceComponent : ComponentBase
{
    [Inject] protected LoadingState Loading { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    public void ShowSnake(SeverityMessage severityMessage)
    {
        if (!string.IsNullOrEmpty(severityMessage.vchMessage))
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add(severityMessage.vchMessage, 
                MudBlazorConverter.ConvertSeverity(severityMessage.Severity));
        }
    }
}
```

### EngramaPage (Base para Páginas)

Las páginas heredan de `EngramaPage` que extiende `EngramaWorkspaceComponent`:

```csharp
public class EngramaPage : EngramaWorkspaceComponent
{
    [Inject] protected IValidaServicioService validaServicioService { get; set; }
    [Inject] protected IHttpService httpService { get; set; }
    [Inject] protected MapperHelper mapperHelper { get; set; }
}
```

### EngramaComponent (Base para Componentes)

Los componentes heredan de `EngramaComponent` que extiende `EngramaWorkspaceComponent`:

```csharp
public class EngramaComponent : EngramaWorkspaceComponent
{
}
```

## TipoEstadoControl (Enum de Estados)

El enum `TipoEstadoControl` gestiona el estado de los formularios:

```csharp
public enum TipoEstadoControl
{
    Lectura,    // Solo lectura, sin edición
    Edicion,    // Editando registro existente
    Alta        // Creando nuevo registro
}
```

### Uso en Componentes

```crazor
@* Ocultar formulario en modo lectura *@
@if (Data.EstadoControl != TipoEstadoControl.Lectura)
{
    <FormMiEntidad Data="@Data" />
}

@* Cambiar estado *@
<MudButton OnClick="@(() => Data.EstadoControl = TipoEstadoControl.Alta)">
    Nuevo Registro
</MudButton>
```

## Convenciones Importantes

### Tabs vs Rutas

| Tipo | Uso | Ejemplo |
|------|-----|---------|
| **Tabs** | Módulos CRUD que se abren múltiples veces | Usuarios, Productos, Reportes |
| **Rutas** | Páginas de navegación única | Inicio, Configuración, Ayuda |

### Parámetros con Dictionary

Los parámetros se pasan usando `Dictionary<string, object>` para compatibilidad con `DynamicComponent`:

```csharp
// Registrar con parámetros
WorkspaceService.AgregarComponenteTab(new WorkspaceTab
{
    Titulo = "Mi Módulo",
    ComponentType = typeof(MiComponente),
    Parameters = new Dictionary<string, object>
    {
        { "Data", miInstancia },
        { "Modo", "Edicion" }
    }
});

// Recibir en el componente
[Parameter] public MainMiEntidad Data { get; set; }
[Parameter] public string Modo { get; set; }
```

### Evitar Infinite Loops

Nunca llames a `SetMenuItems` o `StateHasChanged` dentro de `OnParametersSetAsync`:

```csharp
// ❌ MAL - Causa infinite loop
protected override void OnParametersSetAsync()
{
    WorkspaceService.SetMenuItems(...); // Dispara evento → StateHasChanged → loop
}

// ✅ BIEN - Solo se ejecuta una vez
protected override void OnInitializedAsync()
{
    WorkspaceService.SetMenuItems(...); // Se ejecuta una vez al crear
}
```

## Recomendaciones

- **Modularidad**: Mantenga los módulos pequeños y enfocados en una sola funcionalidad para facilitar el mantenimiento.
- **Reutilización**: Aproveche la clase `Main` para centralizar la lógica y evitar duplicación en páginas y componentes.
- **Workspace**: Registre cada módulo CRUD en el `WorkspaceService` para habilitar el sistema de tabs.
- **Genérico**: Nunca agregue código específico de módulos en `MainLayout`; todo se registra dinámicamente.
- **Parámetros**: Use `Dictionary<string, object>` para pasar parámetros a través de `DynamicComponent`.
- **Estado**: Use `TipoEstadoControl` para gestionar el ciclo de vida de los formularios (Lectura → Edición → Alta).
- **Estilo**: Personalice los estilos en `wwwroot/css/app.css` para el efecto aurora y glassmorphism.

## Conclusión

La capa de Cliente en la metodología Engrama, basada en Blazor WebAssembly PWA y MudBlazor, utiliza un sistema de tabs dinámico que permite abrir múltiples módulos simultáneamente. La arquitectura se compone de `WorkspaceService` (gestión de tabs), `DynamicComponent` (renderizado dinámico), y `ToolbarSuperior` (menú dinámico). Los módulos CRUD se registran en el workspace y se renderizan como pestañas, mientras que las páginas de navegación usan rutas regulares. Esta arquitectura proporciona una interfaz de usuario tipo IDE, moderna y escalable.