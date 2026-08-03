# Metodología Engrama: Capa de Cliente (Blazor WebAssembly PWA)

La capa de **Cliente** en la metodología Engrama utiliza **.NET Blazor WebAssembly** en su versión más reciente como tecnología principal para desarrollar una interfaz gráfica moderna, responsiva y funcional, configurada como una **Progressive Web App (PWA)**. Esta capa permite a los usuarios interactuar con la aplicación a través de componentes visuales, aprovechando la biblioteca **MudBlazor** para simplificar el desarrollo de componentes funcionales y estilizados. Este documento detalla la estructura, convenciones y ejemplos para el cliente, asegurando homogeneidad con las capas de Servicio, Dominio e Infraestructura.

## Tecnologías y Herramientas

- **.NET Blazor WebAssembly**: Framework utilizado para crear aplicaciones web interactivas, optimizadas para ejecución en el navegador.
- **MudBlazor**: Biblioteca de componentes que proporciona elementos de interfaz predefinidos, como tablas, formularios y barras de notificación, para acelerar el desarrollo.
- **EngramaCoreStandar**: Paquete/proyecto base. En el cliente se usan principalmente:
  - `IHttpService` — `Get`, `Post`, `Put`, `Delete`, multipart
  - `IValidaServicioService.ValidadionServicio` — `HttpResponseWrapper<Response<T>>` → `SeverityMessage`
  - `MapperHelper` — mapeos en `Main*` si se requieren
  - Extensiones (`IsEmpty`, `NotNull`, …) y `SeverityMessage` para snackbars (`ShowSnake`)
  - Registro: `AddEngramaDependenciesBlazor()` + `HttpClient` con `BaseAddress`
  - Referencia completa: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md) § 6  
  - Guía en la app: ruta `/guia-engrama-core`

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

La arquitectura de tabs permite abrir múltiples módulos simultáneamente en pestañas, similar a un IDE. Hay una separación clara de responsabilidades:

- **`WorkspaceService`**: hub de eventos (abrir tab, menú del toolbar, renombrar, estado, cerrar). **No** posee la lista de pestañas.
- **`MainLayout`**: dueño del estado de las pestañas (`tabsList`, índice activo). Escucha los eventos del servicio y renderiza con `MudTabs` + `DynamicComponent`.

### Flujo del Sistema de Tabs

```
Usuario abre módulo → WorkspaceService.AddComponenteTab(tab)
    → Evento OnMenuTabSelected → MainLayout.AddTab(tab)
    → Deduplicación por identidad (tipo + EntityId + modo, o ForceNewTab)
    → MudTabs + DynamicComponent renderizan cada pestaña abierta
```

Mientras exista al menos una pestaña, `MainLayout` oculta `@Body` (rutas normales). Al cerrar todas, vuelve a mostrarse la página enrutada (por ejemplo Inicio). Esto es intencional: el workspace prioriza el trabajo multi-módulo sobre deep-linking.

### WorkspaceService (Hub de eventos)

El servicio es un **Singleton** en Blazor WASM (un cliente = un usuario). Expone eventos y métodos para que los módulos no dependan de `MainLayout`:

```csharp
public class WorkspaceService
{
    public event Action<WorkspaceTab> OnMenuTabSelected;
    public void AddComponenteTab(WorkspaceTab tab)
        => OnMenuTabSelected?.Invoke(tab);

    public event Action MenuChanged;
    public IReadOnlyList<MenuItemModel> CurrentMenuItems { get; }
    public void SetMenuItems(List<MenuItemModel> menuItems) { /* ... */ }

    public event Action<string, string> TabNameChanged;
    public void SetCurrentTabName(string tabName, string iconText) { /* ... */ }

    public event Action<TipoEstadoControl> TabStateChanged;
    public void SetCurrentTabState(TipoEstadoControl tabState) { /* ... */ }

    public event Action<bool> CloseCurrentTab;
    public void SetCloseCurrentTab(bool withWarning = false) { /* ... */ }
}
```

#### Explicación
- Abrir una pestaña: `AddComponenteTab` → `MainLayout` agrega o activa.
- Toolbar: cada Grid/Form llama `SetMenuItems` en su init; `ToolbarSuperior` lee `CurrentMenuItems`.
- Renombrar / estado / cerrar: eventos que `MainLayout` aplica sobre la pestaña activa (o por `Id` al cerrar desde la X).

### WorkspaceTab (Modelo de Tab)

```csharp
public class WorkspaceTab
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Icono { get; set; }
    public string Text { get; set; }
    public Type TipoControl { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public TipoEstadoControl EstadoControl { get; set; }
    public object EntityId { get; set; }
    public bool ForceNewTab { get; set; }

    /// <summary>
    /// Identidad para deduplicar: TipoControl | EntityId | EstadoControl.
    /// Si ForceNewTab == true, la identidad es única (siempre abre otra pestaña).
    /// EntityId / ForceNewTab NO van en Parameters (DynamicComponent solo acepta [Parameter] del componente).
    /// </summary>
    public string GetIdentityKey() { /* ver implementación en código */ }
}
```

**Parámetros de identidad recomendados**

| Campo | Uso |
|-----------|-----|
| `Parameters["Data"]` | Instancia `Main*` del módulo ([Parameter] del componente) |
| `Parameters["EstadoControl"]` / `EstadoControl` | `Lectura` / `Edicion` / `Alta` |
| `Parameters["Registro"]` | Snapshot de la entidad en el formulario |
| `EntityId` (en el tab) | Id para dedupe (no pasar a DynamicComponent) |
| `ForceNewTab` (en el tab) | `true` fuerza una pestaña nueva (Altas) |

### MainLayout (Dueño del estado + DynamicComponent)

`MainLayout` es genérico: no referencia módulos concretos. Posee `tabsList` y suscribe los eventos del `WorkspaceService` en `OnInitialized`; en `Dispose` debe desuscribir **todos** los handlers.

```razor
@if (tabsList.Count != 0)
{
    <ToolbarSuperior OnCloseTab="CloseCurrentTabAsync" />
    <MudTabs ActivePanelIndex="tabsSelectedIndex" ActivePanelIndexChanged="OnTabChanged">
        @foreach (var tab in tabsList)
        {
            <MudTabPanel>
                <ChildContent>
                    <DynamicComponent Type="tab.TipoControl" Parameters="tab.Parameters" />
                </ChildContent>
                <TabContent>
                    <!-- Título + X que cierra ESTA pestaña (tab.Id), no solo la activa -->
                    <MudIconButton Icon="@Icons.Material.Filled.Close"
                                   OnClick="@(() => CloseTabAsync(tab))" />
                </TabContent>
            </MudTabPanel>
        }
    </MudTabs>
}
else
{
    @Body
}
```

**Deduplicación en `AddTab`**: si ya existe una pestaña con la misma `GetIdentityKey()`, se activa; si no, se agrega. Grids sin `EntityId`/`ForceNewTab` se reutilizan por `TipoControl`. Formularios con `EntityId` + modo pueden coexistir (Ver 5 y Editar 7 a la vez).

**Estado en memoria**: el estado de negocio debe vivir en `Main*` (pasado en `Parameters["Data"]`), no solo en el componente visual, porque paneles inactivos de MudBlazor pueden remontarse.

### ToolbarSuperior (Barra de acciones del módulo activo)

Muestra `WorkspaceService.CurrentMenuItems` (botones del Grid/Form activo) y un botón para cerrar la pestaña actual vía `EventCallback OnCloseTab`.

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
        // Nombre histórico del método en EngramaCoreStandar
        var validation = validaServicioService.ValidadionServicio(response,
            onSuccess: data => LstMiEntidad = data.ToList());

        return validation;
    }

    public async Task<SeverityMessage> PostSaveRegistro()
    {
        var apiUrl = $"{url}/PostSaveRegistro";
        var response = await httpService.Post<MiEntidad, Response<bool>>(apiUrl, EntidadSeleccionada);
        var validation = validaServicioService.ValidadionServicio(response,
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

        // Abrir este módulo en una pestaña del workspace
        WorkspaceService.AddComponenteTab(new WorkspaceTab
        {
            Text = "Mi Entidad",
            Icono = Icons.Material.Filled.TableChart,
            TipoControl = typeof(GridMiEntidad),
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
WorkspaceService.AddComponenteTab(new WorkspaceTab
{
    Text = "Mi Módulo",
    TipoControl = typeof(MiComponente),
    EstadoControl = TipoEstadoControl.Edicion,
    EntityId = 42,
    // ForceNewTab = true,
    Parameters = new Dictionary<string, object>
    {
        { "Data", miInstancia },
        { "EstadoControl", TipoEstadoControl.Edicion }
    }
});

// Recibir en el componente (solo lo que va en Parameters)
[Parameter] public MainMiEntidad Data { get; set; }
[Parameter] public TipoEstadoControl EstadoControl { get; set; }
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

## EngramaCoreStandar en el Cliente (PWA)

Registro en `Program.cs`:

```csharp
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) // o URL de la API
});
builder.Services.AddEngramaDependenciesBlazor();
```

| Pieza | API | Uso en Main* / componentes |
|-------|-----|----------------------------|
| `IHttpService` | `Get`, `Post`, `Put`, `Delete`, multipart | Llamadas tipadas a `api/…` |
| `IValidaServicioService` | `ValidadionServicio` (nombre histórico) | `HttpResponseWrapper` → `SeverityMessage` + `onSuccess` |
| `MapperHelper` | `Get` / `Map` | Mapeos locales si hacen falta |
| `SeverityMessage` | `bResult`, `vchMessage`, `Severity` | Snackbars (`ShowSnake`) |
| Extensiones | `IsEmpty`, `NotNull`, … | Validaciones UI |

```csharp
var response = await httpService.Post<DtoX, Response<List<Entity>>>(apiUrl, model);
var msg = validaServicioService.ValidadionServicio(response,
    onSuccess: data => Lst = data.ToList(),
    ContinueWarning: true,
    ContinueError: false);
ShowSnake(msg);
```

También disponibles desde v1.1.2: `httpService.Put<…>` y `httpService.Delete(url)`.

Catálogo: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md) § 6. Guía en app: `/guia-engrama-core`.

## Recomendaciones

- **Modularidad**: Mantenga los módulos pequeños y enfocados en una sola funcionalidad para facilitar el mantenimiento.
- **Reutilización**: Aproveche la clase `Main` para centralizar la lógica y evitar duplicación en páginas y componentes.
- **Workspace**: Registre cada módulo CRUD en el `WorkspaceService` para habilitar el sistema de tabs.
- **Genérico**: Nunca agregue código específico de módulos en `MainLayout`; todo se registra dinámicamente.
- **Parámetros**: Use `Dictionary<string, object>` solo para `[Parameter]` del componente; identidad de tab (`EntityId`, `ForceNewTab`) vive en `WorkspaceTab`, no en `Parameters`.
- **Estado**: Use `TipoEstadoControl` para gestionar el ciclo de vida de los formularios (Lectura → Edición → Alta).
- **Estilo**: Personalice los estilos en `wwwroot/css/app.css` para el efecto aurora y glassmorphism.

## Conclusión

La capa de Cliente en la metodología Engrama, basada en Blazor WebAssembly PWA y MudBlazor, combina **EngramaCoreStandar** (`IHttpService`, `IValidaServicioService`) con el workspace de tabs (`WorkspaceService`, `DynamicComponent`, `ToolbarSuperior`). Los módulos CRUD se registran en el workspace y se renderizan como pestañas; las páginas de documentación usan rutas regulares.