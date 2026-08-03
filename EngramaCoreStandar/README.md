# Engrama Core (EngramaCoreStandar)

NuGet library for Engrama apps: **Dapper + SQL Server stored procedures** (API) and **HttpClient** helpers (Blazor PWA).

## Install / reference

**From source (this template):** project-reference `EngramaCoreStandar` (version **1.1.2**).

**From NuGet:** `EngramaCoreStandar` — publish 1.1.2 after packing this project.

### DI

```csharp
// API
builder.Services.AddEngramaDependenciesAPI();

// Blazor WASM
builder.Services.AddEngramaDependenciesBlazor();
// also register HttpClient with BaseAddress
```

Connection string key (required for Dapper helpers):

```json
"ConnectionStrings": {
  "EngramaCloudConnection": "Server=...;Database=...;..."
}
```

## Dapper helpers — which method to use

| Helper | SP receives TVP list? | Returns |
|--------|----------------------|---------|
| `GetAsync<TResult, TRequest>` | No | 1 row |
| `GetAllAsync<TResult, TRequest>` | No | N rows |
| `GetWithListAsync<TResult, TRequest>` | Yes (`IEnumerable<DT*>`) | 1 row |
| `GetAllWithListAsync<TResult, TRequest>` | Yes | N rows |

**TVP rules**

1. SQL: `CREATE TYPE dbo.DTMyType AS TABLE (...)` and SP param `@Items DTMyType READONLY`.
2. C#: class named exactly `DTMyType` with matching columns.
3. `Request` property: `IEnumerable<DTMyType> Items` (name = SQL param without `@`).
4. Empty lists are supported (schema is built from the element type).

Full Engrama walkthrough: `MetodologiaEngrama/8- Guia Consultas SP.md` in TemplatePWA.

## Servicios (Blazor)

- `IHttpService` — `Get`, `Post`, `Put`, `Delete`, multipart helpers
- `IValidaServicioService` — maps `Response<T>` HTTP wrappers to `SeverityMessage` for UI

## 1.1.2 improvements

- Fixed `BuildSqlCommand` (compile error)
- `GetAllWithListAsync` qualifies SP with schema (`[dbo].[sp…]`)
- Empty TVP DataTables keep column schema
- Safer reader column mapping
- Fixed inverted null check in `ValidateSPResult`
- Script `GetAsync` uses `QueryFirstOrDefaultAsync` (not scalar)
- Added `Put` / `Delete` on `IHttpService`
