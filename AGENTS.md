# TemplatePWA — Engrama Agent Rules (Antigravity / AGENTS.md)

You are working in an Engrama Clean Architecture Blazor WASM PWA template.
This file is the shared source of truth. `GEMINI.md` and `MetodologiaEngrama/` apply only where they do not conflict with it.

> This file is the **cross-tool** rules document for **Antigravity** (and other agents that read `AGENTS.md`).
> Cursor-specific mirrors: `.cursorrules` and `.cursor/rules/`.
> Antigravity notes: `GEMINI.md` (this file wins on conflict) and `.agent/rules/`.

## Mandatory documentation

Before adding or changing features, align with the relevant doc:

| Doc | When |
|-----|------|
| `MetodologiaEngrama/0- EngramaCoreStandar.md` | EngramaCoreStandar APIs (Dapper, Mapper, HTTP, Response) |
| `MetodologiaEngrama/1- Base de Datos.md` | Tables, columns, `DT*` types |
| `MetodologiaEngrama/2- Stored Procedures.md` | `spGet*` / `spSave*`, `bResult` / `vchMessage` |
| `MetodologiaEngrama/3- Solucion base y proyecto Share.md` | Solution layout, Share `Objects` / `DtoModels` |
| `MetodologiaEngrama/4- API y Capra Infraestrucura.md` | Repository, Entities `sp*`, `IDapperManagerHelper` |
| `MetodologiaEngrama/5- Dominio.md` | Domain, `MapperHelper`, `IResponseHelper` |
| `MetodologiaEngrama/6- Servicio.md` | Controllers — thin, return `Response<T>`. Route names follow [HTTP endpoint names](#http-endpoint-names), not the DTO-class examples in that doc |
| `MetodologiaEngrama/7- Cliente.md` | PWA modules, `Main*`, workspace tabs |
| `MetodologiaEngrama/8- Guia Consultas SP.md` | Which Dapper helper to use |

Package spelling: **EngramaCoreStandar** (not “Standard”).

## Clean Architecture (Engrama levels)

Respect dependency direction. Never skip or invert layers:

```
PWA (UI) → Share DTOs
       ↘
API Controllers → Domain → Infrastructure → SQL (Stored Procedures)
                      ↘ Share Objects/DtoModels
```

| Layer | Project / folder | Responsibility | Must NOT |
|-------|------------------|----------------|----------|
| DB | `BaseDatos/`, SQL | Tables, UDTT `DT*`, SPs | — |
| Infrastructure | `API/.../Infrastructure` | Call SPs via `IDapperManagerHelper` | Business rules, HTTP |
| Domain | `API/.../Domain` | Map DTO↔Request, `IResponseHelper`, light logic | SQL/Dapper direct, UI |
| Service | `API/.../Controllers` | Endpoints, `Ok`/`BadRequest(Response)` | Business logic, Dapper |
| Share | `TemplatePWA.Share` | `Objects`, `DtoModels` shared contracts | Infrastructure entities |
| Client | `TemplatePWA.PWA` | UI, `Main*`, `IHttpService` | Direct DB access |

Prefer business rules in stored procedures when Engrama style allows; Domain only for orchestration, mapping, and what SPs cannot do.

## SOLID (always apply)

- **S** — One reason to change per class (one module Domain/Repository/Controller/Main).
- **O** — Extend with new modules/SPs; do not hack unrelated modules.
- **L** — Repository/Domain implement their interfaces (`I*Repository`, `I*Domain`).
- **I** — Keep interfaces focused per module; no god interfaces.
- **D** — Depend on abstractions (`IDapperManagerHelper`, `I*Domain`, `IHttpService`); register via DI.

## EngramaCoreStandar usage

### API (`AddEngramaDependenciesAPI`)

- Repositories inject **`IDapperManagerHelper` only** (not raw `IDapperManager`).
- Entities: `spName.Request : SpRequest`, `spName.Result : DbResult` (`bResult`, `vchMessage`).
- Helpers:

| Case | Helper |
|------|--------|
| 1 row, no list | `GetAsync` |
| N rows, no list | `GetAllAsync` |
| 1 row + TVP | `GetWithListAsync` |
| N rows + TVP | `GetAllWithListAsync` |

- Domain: `MapperHelper.Get` + `IResponseHelper.Validacion` → `Response<T>`.
- Connection string key: `EngramaCloudConnection`.

### PWA (`AddEngramaDependenciesBlazor`)

- `Main*`: `IHttpService` + `IValidaServicioService.ValidadionServicio` (historical spelling).
- Show `SeverityMessage` via snackbars (`ShowSnake`).
- Workspace: tab identity (`EntityId`, `ForceNewTab`) on `WorkspaceTab` — **never** inside `DynamicComponent` `Parameters`.

## Module workflow (new feature checklist)

1. SQL table / `DT*` / `spGet*` or `spSave*` with `bResult`, `vchMessage`.
2. Share: `Objects` + `DtoModels`.
3. Infrastructure: `DT*` (if TVP) + `sp*` entity + Repository method.
4. Domain: map DTO → Request; assign TVP lists explicitly (never leave null).
5. Controller: thin `POST api/{Controller}/Post{Action}` (the C# method name). The DTO is the `[FromBody]` type, not the route. Return `Ok(result)` / `BadRequest(result)`. See [HTTP endpoint names](#http-endpoint-names).
6. PWA: `Main*` calls that same `Post…` route + Grid/Form; register workspace tab if CRUD.

Match names across SQL ↔ C# properties ↔ DTO fields.

## HTTP endpoint names

Where `GEMINI.md` or `MetodologiaEngrama/` names a route differently, this section wins. The convention is the one ConectaControl uses.

- Controller: `[Route("api/[controller]")]`.
- The action is `[HttpPost("…")]`. The template is the C# method name and starts with `Post`.
- The Share DTO is only the `[FromBody]` type. It is not the route segment.
- A module controller with one read and one write uses `PostGet` and `PostSave` (`api/Producto/PostGet`, `api/Producto/PostSave`; bodies `DtoGetProducto`, `DtoSaveProducto`).
- Extra actions keep the same prefix and the method name (`PostGetListas`, `PostSaveLista`, `PostValidar`). Do not splice the DTO class into the route.
- The Test module to copy uses `PostTestTable`, `PostSaveTest_Table`, and `PostGetTestTableDataType`. Not `DtoTestTable`.
- The client calls that exact path. Do not invent `PostSaveRegistro` or `GetProducts` when the action is `PostSave` or `PostGet`.

Do not copy these older texts when they disagree:

- `MetodologiaEngrama/3- Solucion base y proyecto Share.md`: `[HttpPost("DtoSaveUsuarios")]`.
- `MetodologiaEngrama/6- Servicio.md` prose that sets the method name to the DTO class (`DtoTestTable`). The code samples in that doc that already say `PostTestTable` and `PostSaveTestTable` match this rule; the prose does not.
- `MetodologiaEngrama/7- Cliente.md` samples that call `PostGetRegistros` or `PostSaveRegistro` against a controller whose actions are `PostGet` and `PostSave`.

ConectaControl's only routes without the `Post` prefix are the four `AzureBlobController` uploads (`UploadImage-tickets`, `UploadImage-logos`, `UploadImage-buzon`, `UploadImage-productos`). New endpoints do not copy that shape. There is no `HttpGet`, `HttpPut`, `HttpDelete`, or `HttpPatch` on those controllers.

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    /// <summary>Saves a product. Route: api/Producto/PostSave.</summary>
    [HttpPost("PostSave")]
    public async Task<IActionResult> PostSave([FromBody] DtoSaveProducto postModel)
    {
        var result = await domain.SaveProducto(postModel);
        if (result.IsSuccess) return Ok(result);
        return BadRequest(result);
    }
}
```

The domain method stays `SaveProducto` (no `Post`, no `sp`). The HTTP route is `PostSave`.

## Comments (required)

Always add comments in new/changed code:

- **XML docs** (`///`) on public classes, interfaces, and public methods (purpose, params, returns).
- **Brief `//` comments** on non-obvious logic (TVP mapping, Save insert vs update, workspace identity).
- Comment **why**, not noise that restates the next line.
- SP scripts: header comment with name and purpose.
- Prefer Spanish or English consistently with the surrounding file (this template mixes both; match the file).

Example:

```csharp
/// <summary>
/// Persists Test_Table (insert when iIdTest_Table &lt;= 0; otherwise update).
/// </summary>
public async Task<Response<Test_Table>> SaveTest_Table(DtoSaveTest_Table postModel)
{
    // Map Share DTO to SP request expected by Infrastructure
    var model = mapperHelper.Get<DtoSaveTest_Table, spSaveTest_Table.Request>(postModel);
    var result = await testRepository.spSaveTest_Table(model);
    return responseHelper.Validacion<spSaveTest_Table.Result, Test_Table>(result);
}
```

## Best practices

- Prefer clarity over cleverness; match existing Engrama patterns in the template.
- Do not put secrets in source; use `appsettings` / user secrets / env.
- Do not invent alternate ORMs or skip SPs for Engrama data access.
- Controllers stay thin; no business logic in Razor beyond UI state.
- Use Engrama extensions idiomatically (`IsNull()`, `False()`, `IsEmpty()`) where the codebase already does.
- After SP/TVP changes, keep SQL script (`BaseDatos/ScritpCompleto.sql`) and C# entities in sync.
- Do not create docs the user did not ask for; when changing methodology behavior, update `MetodologiaEngrama/` only if requested or clearly required for consistency.
- Fix root causes; no silent catch-and-ignore.
- Keep diffs focused on the task.
- Before large changes, read the matching `MetodologiaEngrama` doc and the closest existing module (`TestModule`) as the pattern to copy.

## Anti-patterns (reject)

- Calling Dapper/SQL from Domain, Controllers, or PWA.
- Putting `EntityId` / `IsNewTab` / `ForceNewTab` in `DynamicComponent` Parameters.
- Leaving TVP `IEnumerable` null on Request.
- Renaming package to “EngramaCoreStandard”.
- Naming an HTTP route after the DTO class (`DtoSaveProducto`, `DtoSaveUsuarios`, `DtoTestTable`) or dropping the `Post` prefix (`Save`, `GetProducts`).
- Fat controllers or UI that duplicates Domain logic.
- Uncommented public API surface.

## Quality checks (Antigravity)

- After C# changes, prefer verifying with `dotnet build` on the affected project(s).
- Keep Share ↔ API ↔ PWA contracts aligned when changing DTOs.
- If SQL scripts change, call out that the partner must apply them on the Engrama database.
