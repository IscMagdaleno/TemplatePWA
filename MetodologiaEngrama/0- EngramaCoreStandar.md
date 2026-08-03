# Metodología Engrama: EngramaCoreStandar (paquete base)

**EngramaCoreStandar** (v1.1.2 en este template) es la librería compartida de Engrama. Proporciona acceso a SQL Server vía Dapper, cliente HTTP tipado para Blazor, mapeo DTO↔entidades, respuestas estandarizadas y extensiones de utilidad.

En TemplatePWA se referencia por **ProjectReference** (código en `EngramaCoreStandar/EngramaCoreStandar/`), no por NuGet, para poder probar cambios locales. En producción puede publicarse como paquete NuGet.

> Namespace del ensamblado: `EngramaCoreStandar` (ortografía histórica del paquete).

---

## Índice de carpetas del paquete

| Carpeta | Namespace principal | Rol |
|---------|---------------------|-----|
| `Dapper/` | `EngramaCoreStandar.Dapper` | Ejecutar SPs (`IDapperManagerHelper`) |
| `Dapper/Interfaces/` | `EngramaCoreStandar.Dapper.Interfaces` | `SpRequest`, `DbResult` |
| `Dapper/Results/` | `EngramaCoreStandar.Results` / `.Dapper.Results` | `DataResult`, `Response`, `SeverityMessage`, `Error` |
| `Servicios/` | `EngramaCoreStandar.Servicios` | `IHttpService`, `IValidaServicioService` |
| `Mapper/` | `EngramaCoreStandar.Mapper` | `MapperHelper` (AutoMapper) |
| `Extensions/` | `EngramaCoreStandar.Extensions` | DI + helpers de string, fechas, listas, etc. |
| `Logger/` | `EngramaCoreStandar.Logger` | `ILoggerHelper` (consola / validación SP) |
| `General/` | `EngramaCoreStandar` | `Defaults`, `Jsons` |

Documentos relacionados: [1- Base de Datos](1-%20Base%20de%20Datos.md) · [2- Stored Procedures](2-%20Stored%20Procedures.md) · [3- Solucion](3-%20Solucion%20base%20y%20proyecto%20Share.md) · [4- Infraestructura](4-%20API%20y%20Capra%20Infraestrucura.md) · [5- Dominio](5-%20Dominio.md) · [6- Servicio](6-%20Servicio.md) · [7- Cliente](7-%20Cliente.md) · [8- Consultas SP](8-%20Guia%20Consultas%20SP.md).

---

## 1. Registro DI

```csharp
using EngramaCoreStandar.Extensions;

// API (Program.cs)
builder.Services.AddEngramaDependenciesAPI();
// Registra: IResponseHelper, IDapperManager, IDapperManagerHelper,
//           ILoggerHelper, MapperHelper (singleton)

// Blazor WASM (Program.cs)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://api...") });
builder.Services.AddEngramaDependenciesBlazor();
// Registra: IValidaServicioService, IHttpService, ILoggerHelper, MapperHelper
```

**Connection string (API)** — clave fija leída por `DapperMangerHelper`:

```json
"ConnectionStrings": {
  "EngramaCloudConnection": "Server=...;Database=Engrama;..."
}
```

---

## 2. Contratos Dapper: SpRequest y DbResult

Todo SP Engrama se modela en Infrastructure con dos clases anidadas:

```csharp
public class spGetTestTable
{
    public class Request : SpRequest
    {
        public string StoredProcedure { get => "spGetTestTable"; }
        // propiedades = parámetros SQL (excepto StoredProcedure)
    }

    public class Result : DbResult
    {
        public bool bResult { get; set; }
        public string vchMessage { get; set; }
        // columnas del SELECT del SP
    }
}
```

| Interfaz | Miembros | Uso |
|----------|----------|-----|
| `SpRequest` | `string StoredProcedure { get; }` | Nombre exacto del procedimiento |
| `DbResult` | `bool bResult`, `string vchMessage` | Éxito / mensaje del SP |

---

## 3. IDapperManagerHelper — elegir el método

| Método | ¿TVP / lista? | Retorno | Ejemplo template |
|--------|---------------|---------|------------------|
| `GetAsync<TResult, TRequest>(request)` | No | 1 fila (`DataResult<TResult>`) | `spSaveTest_Table` |
| `GetAllAsync<TResult, TRequest>(request)` | No | N filas | `spGetTestTable` |
| `GetWithListAsync<TResult, TRequest>(request)` | Sí | 1 fila | SP batch con un resultado |
| `GetAllWithListAsync<TResult, TRequest>(request)` | Sí | N filas | `spGetTestTableDataType` |
| `GetAsync<TResult>(script)` / `GetAllAsync<TResult>(script)` | — | SQL texto libre | Uso avanzado |

`DataResult<T>`: `Ok`, `Msg`, `Data`, `Ex`, `Error`.

### TVP (listas)

1. SQL: `CREATE TYPE dbo.DTNombre AS TABLE (...)` + `@Items DTNombre READONLY`.
2. C#: clase `DTNombre` (mismo nombre que el tipo SQL).
3. `Request`: `IEnumerable<DTNombre> Items` (mismo nombre que el parámetro).
4. Helper: `GetWithListAsync` o `GetAllWithListAsync`.
5. v1.1.2: lista vacía crea DataTable con columnas (schema válido).

Detalle: [2- Stored Procedures](2-%20Stored%20Procedures.md), [8- Guia Consultas SP](8-%20Guia%20Consultas%20SP.md).

### IDapperManager (capa baja)

Usado internamente por el helper: `GetAsync` / `GetAllAsync` con `DynamicParameters` o script texto. Los repositorios Engrama **no** lo inyectan directamente; usan `IDapperManagerHelper`.

---

## 4. IResponseHelper / ResponseHelper (Dominio)

Convierte resultados `DbResult` del repositorio en `Response<T>` para la API.

```csharp
// Una fila
Response<Test_Table> r = responseHelper.Validacion<spSave.Result, Test_Table>(result);

// Varias filas
Response<IEnumerable<Test_Table>> r =
    responseHelper.Validacion<spGet.Result, Test_Table>(resultList);
```

También existe `ValidacionSinMapper` cuando no se desea mapear a otro tipo.

`Response<T>`:

| Propiedad | Significado |
|-----------|-------------|
| `IsSuccess` | Operación OK |
| `Message` | Mensaje (suele venir de `vchMessage`) |
| `Data` | Payload tipado |
| `BadResult(msg, data)` | Factory de error |

---

## 5. MapperHelper

AutoMapper con registro perezoso de pares de tipos:

```csharp
var request = mapperHelper.Get<DtoSave, spSave.Request>(dto);
var list = mapperHelper.Get<DtoA, EntityB>(listaDto);           // IList
var enumMapped = mapperHelper.GetEnumerable<TIn, TOut>(items);
mapperHelper.Map(origen, destino);                              // sobreescribe destino
```

Registrado como **Singleton** en API y Blazor.

---

## 6. Servicios (Blazor / PWA)

### IHttpService

| Método | Uso |
|--------|-----|
| `Get<T>(url)` | GET tipado |
| `Post<T, TResponse>(url, data)` | POST JSON → `TResponse` (típico: `Response<List<…>>`) |
| `Post<T>(url, data)` | POST sin cuerpo tipado de respuesta |
| `Post<TResponse>(url)` | POST vacío |
| `Post<…>(url, data, headers)` | POST con headers |
| `Put<T, TResponse>(url, data)` | PUT (v1.1.2) |
| `Delete(url)` | DELETE (v1.1.2) |
| `PostWithFile` / `PostWithImage` / `PostDataContentVideo` | Multipart |

Envoltorio: `HttpResponseWrapper<T>` (`Success`, `Response`, `HttpResponseMessage`).

**Nota:** algunos `Post` tratan `BadRequest` como éxito de transporte para poder leer el body `Response<T>` con `IsSuccess = false` (patrón Engrama).

### IValidaServicioService

```csharp
SeverityMessage msg = validaServicioService.ValidadionServicio(
    httpWrapper,
    onSuccess: data => Lst = data.ToList(),
    ContinueWarning: true,
    ContinueError: false);
```

Devuelve `SeverityMessage` (`bResult`, `vchMessage`, `Severity` = Success|Warning|Error|Info) para snackbars (`ShowSnake` en el cliente).

> El nombre del método es `ValidadionServicio` (ortografía histórica del paquete).

---

## 7. Logger — ILoggerHelper

| Método | Uso |
|--------|-----|
| `Info(string)` | Log a consola |
| `ValidateResult` / `ValidateResultLst` | Log de `DataResult` / listas |
| `ValidateSPResult` | Log de un `DbResult` (v1.1.2 corrige null-check) |
| `ValidaLista` / `ValidaObj` | Validación rápida de colecciones/objetos |

Usado internamente por Dapper; también inyectable en servicios propios.

---

## 8. General

### Defaults

```csharp
Defaults.SqlMinDate();          // 1900-01-01 (útil para TVP / fechas SQL)
Defaults.MX_CultureInfo;        // es-MX
```

### Jsons

Serialización Newtonsoft (`Stringify`, etc.) usada por algunas extensiones (`DeepCopy`, comparación).

---

## 9. Extensions (resumen por área)

Usar con `using EngramaCoreStandar.Extensions;`.

| Extensión | Ejemplos de API |
|-----------|-----------------|
| **Object** | `IsNull()`, `NotNull()`, `ToInt()`, `ToDateTime()`, `ToBoolean()`, `SearchProperty` |
| **String** | `IsEmpty()`, `NotEmpty()`, `Capitalize()`, `ToDateMX()`, `Match()`, `UrlEncode()` |
| **Enumerable / List** | `IsEmpty()`, `NotEmpty()`, `SingleOne()`, `ValidaResult()` (sobre `DbResult`) |
| **Bool** | `False()` (niega sin `!`) |
| **Int / Long** | `Between()`, `ZeroOrLess()`, `GetMonth()`, `IsAny()` |
| **Decimal** | `ToDecimalCurrency()`, `Trim(decimales)` |
| **DateTime** | `ToFormatMX()`, `EqualsToDefaultDate()`, `ToNumber()` |
| **Dictionary** | `IsEmpty()`, `GetValueBy()`, `NoContainsKey()` |
| **Byte[]** | `ToBase64()`, `Compress()`, `WriteFile()` |
| **Generic** | `DeepCopy()`, `Clone()`, `AreEquals()`, `Validate()` |
| **Cryptography** | `Encrypt` / `Decrypt` (uso acotado; no sustituye un diseño de seguridad moderno) |
| **DI** | `AddEngramaDependenciesAPI` / `AddEngramaDependenciesBlazor` |

En Dominio es frecuente: `ModeloValidar.IsNull().False()` (doble negación idiomática Engrama).

---

## 10. Flujo completo (API + PWA)

```
PWA Main*
  → IHttpService.Post<Dto, Response<T>>
  → IValidaServicioService.ValidadionServicio → SeverityMessage / UI

API Controller
  → I*Domain
      → MapperHelper.Get<Dto, sp*.Request>
      → I*Repository
          → IDapperManagerHelper.GetAllAsync / GetAsync / Get*WithListAsync
          → SQL SP (bResult, vchMessage)
      → IResponseHelper.Validacion → Response<T>
  → Ok / BadRequest(Response)
```

---

## 11. Checklist al usar el paquete en un módulo nuevo

1. Share: Objects + DtoModels del módulo.
2. Entity `sp*` con `Request : SpRequest` y `Result : DbResult`.
3. Si hay lista: `DT*` + `Get*WithListAsync`.
4. Repository inyecta `IDapperManagerHelper`.
5. Domain inyecta repo + `MapperHelper` + `IResponseHelper`.
6. Controller delgado; endpoint nombrado como el DTO/proceso.
7. PWA `Main*` con `IHttpService` + `IValidaServicioService`.
8. Connection string `EngramaCloudConnection` configurada.

---

## 12. Cambios relevantes v1.1.2 (template local)

- Compilación de `BuildSqlCommand` corregida.
- `GetAllWithListAsync` califica SP como `[schema].[nombre]`.
- TVP con lista vacía mantiene schema de columnas.
- Mapeo de reader más tolerante a columnas faltantes.
- `ValidateSPResult` null-check corregido.
- Script `GetAsync` usa `QueryFirstOrDefaultAsync`.
- `Put` / `Delete` en `IHttpService`.

Guía interactiva en la PWA: ruta `/guia-engrama-core`.
