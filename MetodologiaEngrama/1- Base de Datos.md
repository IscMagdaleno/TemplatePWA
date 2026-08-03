# Metodología Engrama para el Desarrollo de Software: Creación de Tablas en la Base de Datos

La metodología Engrama establece un conjunto de directrices claras y estructuradas para el diseño y creación de bases de datos, con un enfoque particular en la definición de tablas. Este documento detalla las convenciones para la creación de tablas en SQL Server, asegurando consistencia, claridad y escalabilidad en el desarrollo de software.

> **EngramaCoreStandar:** las columnas y prefijos aquí definidos deben coincidir con las propiedades de las clases `Result` / `DT*` en Infrastructure. Ver [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md). Tipos tabla (`DT*`) → sección User-Defined Table Types más abajo y doc [2-](2-%20Stored%20Procedures.md).

## Convenciones para la Creación de Tablas

1. **Nombre de la Tabla**:  
   - El nombre de la tabla debe reflejar su propósito de manera clara y concisa, utilizando sustantivos en singular.  
   - Ejemplo: `Usuario`, `Producto`, `Orden`.

2. **Estructura de las Columnas**:  
   - Cada tabla debe incluir una columna como clave primaria con el nombre `iId[NombreTabla]`, configurada como `IDENTITY(1,1)` para generar valores automáticos.  
   - Todas las columnas deben definirse como `NOT NULL` para garantizar la integridad de los datos.  
   - Los nombres de las columnas deben seguir un formato estandarizado:  
     - **Prefijo**: Indica el tipo de dato, utilizando letras minúsculas (por ejemplo, `i` para `INTEGER`, `vch` para `VARCHAR`, `nvch` para `NVARCHAR`, etc.).  
     - **Nombre del Campo**: Debe comenzar con la primera letra en mayúscula y describir claramente el contenido del campo.  
     - Ejemplo: `vchNombre`, `dtFechaCreacion`, `mPrecio`.

3. **Prefijos para Tipos de Datos**:  
   - `i`: INTEGER  
   - `vch`: VARCHAR  
   - `nvch`: NVARCHAR  
   - `dt`: DATETIME  
   - `m`: MONEY  
   - `sm`: SMALLINT  
   - `bi`: BIGINT  
   - `fl`: FLOAT  
   - `d`: DATE  
   - `b`: BIT  

4. **Validación de Existencia**:  
   - Antes de crear una tabla, el script SQL debe incluir una verificación para evitar errores si la tabla ya existe en la base de datos.

## Ejemplo de Script SQL para Crear una Tabla

A continuación, se proporciona un ejemplo de un script SQL para crear una tabla llamada `Producto`, siguiendo las convenciones de la metodología Engrama. Este script incluye la validación de existencia y la definición de columnas con los prefijos correspondientes.

 ```sql
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Producto')
BEGIN
    CREATE TABLE Producto
    (
        iIdProducto INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        vchNombre VARCHAR(100) NOT NULL,
        nvchDescripcion NVARCHAR(500) NOT NULL,
        mPrecio MONEY NOT NULL,
        dtFechaCreacion DATETIME NOT NULL,
        bActivo BIT NOT NULL
    );
END;
 ```

## User-Defined Table Types (`DT*`) — Enviar listas a SQL

Cuando un procedimiento necesita recibir **una lista de filas** desde la API (filtros por muchos IDs, altas masivas, etc.), Engrama usa un **User-Defined Table Type** (UDTT) en SQL Server. En código C# la lista se mapea a una clase con prefijo `DT`.

### Convenciones

1. **Nomenclatura**: `DT` + nombre descriptivo en singular/PascalCase. Ejemplo: `DTParameterType`, `DTIdList`, `DTProductoItem`.
2. **Columnas**: mismos prefijos de tipo que las tablas (`i`, `vch`, `dt`, `m`, `sm`, `b`, …).
3. **Orden**: el tipo se crea **antes** de los procedimientos que lo usan.
4. **Paridad C#**: cada columna del tipo debe existir como propiedad en la clase `DT*` de Infrastructure (y normalmente también en Share si viaja en el DTO).

### Ejemplo (script de plantilla)

Del archivo `BaseDatos/ScritpCompleto.sql`:

```sql
CREATE TYPE [dbo].[DTParameterType] AS TABLE(
    [iParameter]   [int] NULL,
    [vchParameter] [varchar](255) NULL,
    [dtParameter]  [datetime] NULL,
    [mParametr]    [money] NULL,
    [smParameter]  [smallint] NULL,
    [bParameter]   [bit] NULL
);
```

Crear tipos específicos del dominio cuando el genérico no baste:

```sql
IF NOT EXISTS (SELECT 1 FROM sys.types WHERE name = 'DTIdList' AND is_table_type = 1)
BEGIN
    CREATE TYPE dbo.DTIdList AS TABLE
    (
        iId INT NOT NULL
    );
END
```

Los procedimientos reciben el parámetro como `@NombreLista DTTipo READONLY`. Detalle completo en `2- Stored Procedures.md`.

### Relación con EngramaCoreStandar

| SQL | C# (Infrastructure) | Helper |
|-----|---------------------|--------|
| Tabla / columnas | `Result` / Objects Share | — |
| `CREATE TYPE DT*` | clase `DT*` | `GetWithListAsync` / `GetAllWithListAsync` |
| Prefijos `i`, `vch`, `dt`, … | mismas propiedades | mapeo Dapper por nombre |
| Fechas “vacías” en TVP | `Defaults.SqlMinDate()` (1900-01-01) | evita null en columnas NOT NULL del tipo |

Catálogo del paquete: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md).