# Domain (Engrama) — Antigravity

Applies when working under API Domain (`**/Domain/**`).

Read `MetodologiaEngrama/5- Dominio.md`.

## Rules

- Inject `I*Repository`, `MapperHelper`, `IResponseHelper`.
- Method names drop `sp` prefix (e.g. `spSaveTest_Table` → `SaveTest_Table`).
- Input: Share `DtoModel`; output: `Response<T>` / `Response<IEnumerable<T>>`.
- Map with `mapperHelper.Get`; validate with `responseHelper.Validacion`.
- Assign TVP lists on Request explicitly after map — never leave null.
- No Dapper, SQL, or HTTP here.
- XML comments on `I*Domain` and public Domain methods; comment non-obvious mapping.
