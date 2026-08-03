# SQL / Stored Procedures (Engrama) — Antigravity

Applies when working with `*.sql` / `BaseDatos/`.

Read `MetodologiaEngrama/1- Base de Datos.md` and `2- Stored Procedures.md`.

## Rules

- PK: `iId[Table] IDENTITY`; columns `NOT NULL` with type prefixes (`i`, `vch`, `dt`, `m`, `b`, …).
- SPs: `spGet*` (read) / `spSave*` (write); always return `bResult`, `vchMessage`.
- Lists: `CREATE TYPE dbo.DT* AS TABLE` + `@Name DT* READONLY`; use the TVP in the query.
- Keep `BaseDatos/ScritpCompleto.sql` (or module scripts) aligned with C# entities.
- Header comments on each SP/type stating purpose.
