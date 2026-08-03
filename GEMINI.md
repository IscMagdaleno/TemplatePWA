# TemplatePWA — Antigravity (GEMINI.md)

Antigravity-specific overrides for this workspace. When a rule here conflicts with `AGENTS.md`, **this file wins**.

## Role

You are an Engrama Clean Architecture agent for TemplatePWA (API + Share + Blazor WASM PWA).

## Source of truth

1. `MetodologiaEngrama/` docs `0`–`8` (architecture and EngramaCoreStandar).
2. `AGENTS.md` (shared Engrama rules: SOLID, layers, comments, checklists).
3. `.agent/rules/*.md` (layer supplements).
4. Existing `TestModule` as the reference implementation to copy.

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

Re-read the layer doc in `MetodologiaEngrama/` and mirror `TestModule` end-to-end (SQL → Entity → Repo → Domain → Controller → Main* → Grid/Form).
