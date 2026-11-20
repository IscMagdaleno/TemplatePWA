# TemplatePWA — Antigravity (GEMINI.md)

Antigravity-specific notes for this workspace. Shared Engrama rules live in `AGENTS.md`. When a rule here conflicts with `AGENTS.md`, **`AGENTS.md` wins**.

## Role

You are an Engrama Clean Architecture agent for TemplatePWA (API + Share + Blazor WASM PWA).

## Source of truth

1. `AGENTS.md` (shared Engrama rules: SOLID, layers, comments, checklists, HTTP endpoint names).
2. `MetodologiaEngrama/` docs `0`–`8`, only where they do not conflict with `AGENTS.md`.
3. `.agent/rules/*.md` (layer supplements).
4. Existing `TestModule` as the reference implementation to copy, using the route rule below.

## HTTP endpoint names

Same rule as `AGENTS.md`:

- `POST api/{Controller}/{Action}`.
- The C# method starts with `Post` and that name is the `[HttpPost]` template. Controller route: `[Route("api/[controller]")]`.
- The DTO is the `[FromBody]` body, not the route.
- One read and one write on a module controller: `PostGet` and `PostSave` (bodies such as `DtoGetProducto` and `DtoSaveProducto`).
- Do not name the route after the DTO class (`DtoSaveUsuarios`, `DtoTestTable`) and do not call `PostSaveRegistro` or `GetProducts` when the action is `PostSave` or `PostGet`.
- Test module actions to copy: `PostTestTable`, `PostSaveTest_Table`, `PostGetTestTableDataType`.

## Behavior

- Follow Clean Architecture Engrama levels; never call Dapper/SQL from Domain, Controllers, or PWA.
- Always add XML/`//` comments on public APIs and non-obvious logic.
- Use **EngramaCoreStandar** spelling and APIs (`IDapperManagerHelper`, `MapperHelper`, `IResponseHelper`, `IHttpService`, `ValidadionServicio`).
- Prefer extending with new modules/SPs over rewriting unrelated code.
- Do not invent alternate data-access stacks (EF for Engrama CRUD paths, raw ADO outside helpers, etc.).
- Do not commit secrets; do not edit generated or binary assets unless asked.

## Partner workflow notes

- Cursor teammates use `.cursorrules` / `.cursor/rules/` — keep Engrama rules aligned when changing methodology behavior.
- Shared portable rules live in `AGENTS.md`; put only Antigravity-specific behavior here.
- Interactive guide in the PWA: route `/guia-engrama-core`.

## When stuck

Re-read `AGENTS.md` first, then the layer doc in `MetodologiaEngrama/` where it does not conflict, and mirror `TestModule` end-to-end (SQL → Entity → Repo → Domain → Controller → Main* → Grid/Form). The controller route follows the HTTP endpoint names rule above.
