# Controllers (Engrama) — Antigravity

Applies when working under API Controllers (`**/Controllers/**`).

Read `MetodologiaEngrama/6- Servicio.md`.

## Rules

- Inherit `ControllerBase`; `[ApiController]`, `[Route("api/[controller]")]`.
- Inject `I*Domain` only — no repository, Dapper, or MapperHelper.
- Prefer `[HttpPost]` Engrama style; body = Share DTO.
- Return `Ok(result)` when `result.IsSuccess`, else `BadRequest(result)`.
- Keep actions thin: one Domain call + status mapping.
- XML docs on every endpoint (summary, param, returns).
