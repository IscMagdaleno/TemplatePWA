# Guía Engrama: Consultas a la base de datos (Get / GetAll / GetWithList / Save)

Esta guía resume **qué helper de EngramaCoreStandar usar** según el tipo de procedimiento almacenado, con el módulo `TestModule` de TemplatePWA como ejemplo listo para copiar.

Referencia completa del paquete (DI, Mapper, HTTP, Extensions, Results): **[0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md)**.  
Script SQL de demo: [`BaseDatos/ScritpCompleto.sql`](../BaseDatos/ScritpCompleto.sql).  
Guía interactiva en la PWA: `/guia-engrama-core`.

---

## Tabla rápida

| Caso | SP ejemplo | Helper EngramaCore | Repo / Domain en template |
|------|------------|--------------------|---------------------------|
| **GetAll** — varios registros, sin lista | `spGetTestTable` | `GetAllAsync` | `TestRepository.spGetTestTable` → `TestDomain.TestTable` |
| **Get** — un registro, sin lista | `spGetTestTableOneResult` / `spSaveTest_Table` | `GetAsync` | `spSaveTest_Table` usa `GetAsync` |
| **GetAll + lista (TVP)** | `spGetTestTableDataType` | `GetAllWithListAsync` | `spGetTestTableDataType` |
| **Get + lista (TVP)** | mismo patrón, 1 fila de resultado | `GetWithListAsync` | (mismo entity, otro helper) |
| **Save** — alta/edición | `spSaveTest_Table` | `GetAsync` (retorna 1 fila con `iId…`) | `SaveTest_Table` |

Flujo completo:

```
PWA (Main*) → POST api/Controller → Domain → Repository → IDapperManagerHelper → SQL SP
```

---

## 1. GetAll — listar (sin TVP)

**SQL:** `spGetTestTable` (sin parámetros de lista).

**Entity** (`Infrastructure/Entity/spGetTestTable.cs`):

```csharp
public class Request : SpRequest
{
    public string StoredProcedure { get => "spGetTestTable"; }
}
```

**Repository:**

```csharp
var respuesta = await _managerHelper
    .GetAllAsync<spGetTestTable.Result, spGetTestTable.Request>(daoModel, "");
```

**Cliente PWA:** `MainTest.PostGetRegistros()` → `POST api/Test/PostTestTable`.

En la UI: al abrir el tab **Catálogo de Test Table** se carga el grid.

---

## 2. Get — un registro (sin TVP)

Usar `GetAsync` cuando el SP devuelve **una sola fila** (detalle o Save).

**Save ejemplo** — `spSaveTest_Table`:

```csharp
var result = await _managerHelper
    .GetAsync<spSaveTest_Table.Result, spSaveTest_Table.Request>(postModel);
```

**Cliente:** formulario **Agregar / Editar** → `MainTest.PostSaveRegistro` → `POST api/Test/PostSaveTest_Table`.

En Alta, `iIdTest_Table = 0`; el SP inserta y devuelve el nuevo Id.

---

## 3. GetAllWithList — filtrar enviando una lista (TVP / DataType)

**SQL**

1. Tipo: `CREATE TYPE dbo.DTParameterType AS TABLE (...)`
2. SP: `@Parameters DTParameterType READONLY` — y **usar** `@Parameters` en el `WHERE` / `JOIN`.

**C#**

1. Clase `DTParameterType` (mismas columnas que el tipo SQL).
2. `Request.Parameters` = `IEnumerable<DTParameterType>`.
3. Repositorio:

```csharp
await _managerHelper
    .GetAllWithListAsync<spGetTestTableDataType.Result, spGetTestTableDataType.Request>(postModel);
```

**Cliente:** botón **Filtrar por TVP** → `MainTest.PostGetByDataType(ids)` → `POST api/Test/PostGetTestTableDataType`.

Envía los primeros IDs del grid como filas `iParameter`; el SP filtra `Test_Table` por esos Ids. Lista vacía / sin Ids > 0 → todos (comportamiento demo del script).

Documentación extendida: docs `1-`, `2-`, `4-`, `5-` (secciones DataType / TVP).

---

## 4. Save desde el formulario (Alta / Edición)

1. Toolbar **Agregar** o iconos Ver/Editar en el grid → abre tab `FormTestTable`.
2. Completar Nombre, Email, Fecha.
3. **Guardar** (toolbar o botón del formulario) → `spSaveTest_Table`.
4. Tras éxito se refresca el catálogo y se puede cerrar el tab.

Estados: `TipoEstadoControl.Alta | Edicion | Lectura`.

---

## Checklist al agregar un SP nuevo

1. Script SQL (`spGet…` / `spSave…`) con `bResult` + `vchMessage`.
2. Si hay lista: `DT*` en SQL + clase `DT*` en Infrastructure (+ Share si viaja en el DTO).
3. Entity `sp…` con `Request : SpRequest` y `Result : DbResult`.
4. Elegir helper de la tabla rápida.
5. Domain: build `Request` manually when TVP types differ (Share `DT*` ≠ Infrastructure `DT*`); never `mapperHelper.Get` the full DTO in that case.
6. Controller delgado + DTO en Share con el mismo nombre de endpoint.
7. `Main*` en PWA con `IHttpService.Post` + `IValidaServicioService`.

---

## Mejoras EngramaCoreStandar 1.1.2 (relevante a esta guía)

- TVP con lista vacía ya no rompe el parámetro Structured.
- `GetAllWithListAsync` usa `[schema].[SP]`.
- Mapeo de columnas del reader más tolerante.
- Ver `EngramaCoreStandar/README.md`.

## Mapa completo EngramaCoreStandar (capa a capa)

| Capa | Funciones del paquete |
|------|------------------------|
| SQL / docs 1–2 | Contratos que alimentan `SpRequest`/`DbResult`/`DT*` |
| Infrastructure | `IDapperManagerHelper`, `DataResult<T>` |
| Dominio | `MapperHelper`, `IResponseHelper`, Extensions |
| Controllers | Solo `Response<T>` |
| PWA | `IHttpService`, `IValidaServicioService`, `SeverityMessage` |

Referencia detallada de cada API: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md).

