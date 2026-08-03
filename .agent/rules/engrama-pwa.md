# Client PWA (Engrama) — Antigravity

Applies when working under `TemplatePWA.PWA` (`.cs` / `.razor`).

Read `MetodologiaEngrama/7- Cliente.md`.

## Rules

- Module API calls live in `Modules/*/Utiles/Main*.cs` via `IHttpService`.
- Validate with `IValidaServicioService.ValidadionServicio` (historical name); surface `SeverityMessage`.
- CRUD UI: Grid + Form; state via `TipoEstadoControl`.
- Workspace: `EntityId` / `ForceNewTab` on `WorkspaceTab` only — never in `DynamicComponent` `Parameters`.
- `Parameters` only for real component `[Parameter]`s (`Data`, `EstadoControl`, etc.).
- No direct database access from PWA.
- XML/`//` comments on `Main*` public methods and non-obvious UI/workspace logic.
