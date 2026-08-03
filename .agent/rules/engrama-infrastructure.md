# Infrastructure (Engrama) — Antigravity

Applies when working under API Infrastructure (`**/Infrastructure/**`).

Read `MetodologiaEngrama/4- API y Capra Infraestrucura.md` and `8- Guia Consultas SP.md`.

## Rules

- Inject `IDapperManagerHelper` only (not `IDapperManager`).
- One `sp*` class per procedure: `Request : SpRequest`, `Result : DbResult`.
- Property names must match SQL parameters/columns exactly.
- TVP: `IEnumerable<DT*>` property name = SQL parameter name; use `GetWithListAsync` / `GetAllWithListAsync`.
- On helper failure, return a `Result` with `bResult = false` and `vchMessage = respuesta.Msg`.
- XML comments on repository interfaces and public methods.
