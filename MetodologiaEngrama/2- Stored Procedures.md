# Metodología Engrama: Estructura de Procedimientos Almacenados

La metodología Engrama define un estándar riguroso para la creación de procedimientos almacenados (SPs) en la base de datos, garantizando homogeneidad, trazabilidad y compatibilidad con las entidades de la API. Este documento detalla la estructura, convenciones y ejemplos para los procedimientos almacenados, asegurando que sean consistentes y fáciles de integrar con la capa de infraestructura.

> **EngramaCoreStandar:** cada SP se invoca desde C# con `IDapperManagerHelper` (`GetAsync`, `GetAllAsync`, `GetWithListAsync`, `GetAllWithListAsync`). Los campos `bResult` / `vchMessage` mapean a `DbResult`. Guía completa del paquete: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md). Tabla de helpers: [8- Guia Consultas SP.md](8-%20Guia%20Consultas%20SP.md).

## Tipos de Procedimientos Almacenados

Engrama clasifica los procedimientos almacenados en dos categorías, según su propósito:

1. **Get**:  
   - Diseñados para consultar y recuperar datos de la base de datos.  
   - Ejemplo: Obtener un listado de usuarios o detalles específicos de una entidad.

2. **Save**:  
   - Utilizados para operaciones de escritura, como inserción, actualización o eliminación de datos.  
   - Ejemplo: Crear o modificar la información de un usuario.

Ambos tipos comparten una estructura interna estandarizada que facilita la integración con las clases de entidades en la API y asegura consistencia en el manejo de datos.

## Convenciones para Procedimientos Almacenados

1. **Nomenclatura**:  
   - Los procedimientos comienzan con el prefijo `sp`, seguido del tipo (`Get` o `Save`) y el nombre del proceso, que refleja su propósito.  
   - Ejemplo: `spGetConnectionString` (para consulta), `spSaveUsers` (para escritura).

2. **Validación de Existencia**:  
   - Antes de crear o modificar un procedimiento, se verifica su existencia con `IF OBJECT_ID('nombre') IS NULL`. Si no existe, se crea un procedimiento vacío que luego se actualiza con `ALTER PROCEDURE`.

3. **Estructura General**:  
   - **Parámetros de Entrada**: Se definen al inicio, utilizando prefijos de tipo de datos (por ejemplo, `i` para `INT`, `vch` para `VARCHAR`) según las convenciones de Engrama.  
   - **Comentarios**: Incluyen el creador, propósito, última actualización y fecha.  
   - **Tabla de Resultado**:  
     - Para procedimientos `Get`, se usa una tabla temporal (`#Result`).  
     - Para procedimientos `Save`, se usa una variable de tabla (`@Result`).  
     - Ambas incluyen los campos obligatorios `bResult` (BIT, indica éxito o fallo) y `vchMessage` (VARCHAR, mensaje de error o nota).  
   - **Control de Errores**: Un bloque `TRY-CATCH` captura errores y los devuelve en `vchMessage`.  
   - **Transacciones**: En procedimientos `Save`, se implementan transacciones para garantizar la integridad de los datos, con rollback en caso de error.  
   - **Retorno**: Los resultados se devuelven mediante un `SELECT` de la tabla de resultado.  
   - **Limpieza**: Las tablas temporales se eliminan con `DROP TABLE` al final.  
   - **Finalización**: El script termina con `GO` y, si es posible, incluye ejemplos de ejecución (`EXEC`) para facilitar pruebas.

4. **Campos Obligatorios**:  
   - Todos los procedimientos deben incluir `bResult` (`1` para éxito, `0` para fallo) y `vchMessage` (mensaje descriptivo).  
   - Los campos devueltos deben coincidir exactamente con las propiedades de la clase `Result` en las entidades de la API.

## Ejemplo de Procedimiento Almacenado: Tipo Get

El siguiente ejemplo muestra un procedimiento de tipo `Get` para consultar información de la tabla `ConnectionString`:

 ```sql
IF OBJECT_ID('spGetConnectionString') IS NULL
    EXEC ('CREATE PROCEDURE spGetConnectionString AS SET NOCOUNT ON;')
GO

ALTER PROCEDURE spGetConnectionString
(
    @iIdUser INT,
    @bActivo BIT
)
AS
/*
** Creador:      Magdaleno Martinez
** Propósito:    Obtener los detalles de una cadena de conexión según el ID de usuario y su estado activo.
** Actualizado por: Alan
** Última fecha: 10/15/2025
*/
BEGIN
    -- Crear tabla temporal para los resultados
    CREATE TABLE #Result
    (
        bResult BIT DEFAULT (1),
        vchMessage VARCHAR(500) DEFAULT (''),
        iIdConnectionString INT DEFAULT (-1),
        iIdUser INT DEFAULT (-1),
        vchConnectionString VARCHAR(250) DEFAULT (''),
        vchNota VARCHAR(250) DEFAULT (''),
        bActivo BIT DEFAULT (0)
    );

    SET NOCOUNT ON;

    BEGIN TRY
        -- Insertar resultados en la tabla temporal
        INSERT INTO #Result
        (
            iIdConnectionString, iIdUser, vchConnectionString,
            vchNota, bActivo
        )
        SELECT
            CS.iIdConnectionString, CS.iIdUser, CS.vchConnectionString,
            CS.vchNota, CS.bActivo
        FROM
            dbo.ConnectionString CS WITH(NOLOCK)
        WHERE
            CS.iIdUser = @iIdUser
            AND (CS.bActivo = @bActivo OR @bActivo = 0);

        -- Validar si no se encontraron registros
        IF NOT EXISTS (SELECT 1 FROM #Result)
        BEGIN
            INSERT INTO #Result (bResult, vchMessage)
            VALUES (0, 'No se encontraron registros.');
        END
    END TRY
    BEGIN CATCH
        -- Capturar error
        INSERT INTO #Result (bResult, vchMessage)
        VALUES (0, CONCAT(ERROR_PROCEDURE(), ': ', ERROR_MESSAGE(), ' - Línea ', ERROR_LINE()));
        
        PRINT CONCAT(ERROR_PROCEDURE(), ': ', ERROR_MESSAGE(), ' - Línea ', ERROR_LINE());
    END CATCH;

    -- Devolver resultados
    SELECT * FROM #Result;

    -- Eliminar tabla temporal
    DROP TABLE #Result;
END
GO

-- Ejemplo de ejecución
EXEC spGetConnectionString @iIdUser = 2, @bActivo = 0;
EXEC spGetConnectionString @iIdUser = 2, @bActivo = 1;
GO

 ```

## Procedimientos que reciben listas (Table-Valued Parameters / DataType)

Además de parámetros escalares, Engrama permite enviar **colecciones** a SQL mediante un User-Defined Table Type (`DT*`). En la API eso se consume con `GetWithListAsync` / `GetAllWithListAsync` (ver `4- API y Capra Infraestrucura.md`).

### Reglas

1. Declarar el parámetro como `@NombreLista DTTipo READONLY` (siempre `READONLY`).
2. El **nombre del parámetro** (`@Parameters`) debe coincidir con la propiedad `IEnumerable<DT*>` del `Request` en C# (`Parameters`).
3. Usar la lista dentro del SP con `JOIN`, `EXISTS` o `INSERT … SELECT` desde la tabla-parámetro.
4. Seguir devolviendo `bResult` + `vchMessage` como en cualquier otro SP Engrama.

### Ejemplo Get con lista — `spGetTestTableDataType`

Script de referencia: `BaseDatos/ScritpCompleto.sql`.

```sql
CREATE PROCEDURE [dbo].[spGetTestTableDataType]
    @iIdTest_Table INT,
    @vchName VARCHAR(50),
    @vchEmail VARCHAR(50),
    @dtRegistered DATETIME,
    @Parameters DTParameterType READONLY   -- lista enviada desde la API
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Result TABLE
    (
        bResult BIT DEFAULT (1),
        vchMessage VARCHAR(500) DEFAULT(''),
        iIdTest_Table INT DEFAULT (-1),
        vchName VARCHAR(100) DEFAULT (''),
        vchEmail VARCHAR(100) DEFAULT (''),
        dtRegistered DATETIME DEFAULT ('1900-01-01')
    );

    BEGIN TRY
        -- Si la lista trae IDs (iParameter > 0), filtrar por ellos;
        -- si viene vacía, devolver todos (demo).
        INSERT INTO @Result (iIdTest_Table, vchName, vchEmail, dtRegistered)
        SELECT TT.iIdTest_Table, TT.vchName, TT.vchEmail, TT.dtRegistered
        FROM dbo.Test_Table TT WITH (NOLOCK)
        WHERE
            NOT EXISTS (SELECT 1 FROM @Parameters P WHERE P.iParameter > 0)
            OR EXISTS (
                SELECT 1
                FROM @Parameters P
                WHERE P.iParameter = TT.iIdTest_Table
            );

        IF NOT EXISTS (SELECT 1 FROM @Result WHERE iIdTest_Table != -1)
        BEGIN
            INSERT INTO @Result (bResult, vchMessage)
            VALUES (0, 'No se encontraron registros.');
        END
    END TRY
    BEGIN CATCH
        INSERT INTO @Result (bResult, vchMessage)
        SELECT 0, CONCAT(ERROR_PROCEDURE(), ' : ', ERROR_MESSAGE(), ' - ', ERROR_LINE());
    END CATCH;

    SELECT * FROM @Result;
END
GO
```

### Ejemplo de ejecución en SSMS

```sql
DECLARE @Lista AS DTParameterType;

INSERT INTO @Lista (iParameter, vchParameter, dtParameter, mParametr, smParameter, bParameter)
VALUES
    (1, 'filtro-a', '1900-01-01', 0, 0, 0),
    (2, 'filtro-b', '1900-01-01', 0, 0, 0);

EXEC dbo.spGetTestTableDataType
    @iIdTest_Table = 0,
    @vchName = '',
    @vchEmail = '',
    @dtRegistered = '1900-01-01',
    @Parameters = @Lista;
```

### Ejemplo Save con lista (patrón batch)

Útil para insertar/actualizar varios ítems en una sola llamada:

```sql
-- Tipo
CREATE TYPE dbo.DTTest_TableItem AS TABLE
(
    iIdTest_Table INT NOT NULL,
    vchName VARCHAR(100) NOT NULL,
    vchEmail VARCHAR(100) NOT NULL,
    dtRegistered DATETIME NOT NULL
);
GO

ALTER PROCEDURE dbo.spSaveTest_TableBatch
    @Items DTTest_TableItem READONLY
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @vchError VARCHAR(200) = '';
    DECLARE @Result TABLE (bResult BIT DEFAULT(1), vchMessage VARCHAR(500) DEFAULT(''), iRowsAffected INT DEFAULT(0));

    BEGIN TRY
        BEGIN TRANSACTION;

        MERGE dbo.Test_Table AS T
        USING @Items AS S
            ON T.iIdTest_Table = S.iIdTest_Table AND S.iIdTest_Table > 0
        WHEN MATCHED THEN
            UPDATE SET vchName = S.vchName, vchEmail = S.vchEmail, dtRegistered = S.dtRegistered
        WHEN NOT MATCHED THEN
            INSERT (vchName, vchEmail, dtRegistered)
            VALUES (S.vchName, S.vchEmail, S.dtRegistered);

        INSERT INTO @Result (bResult, vchMessage, iRowsAffected)
        VALUES (1, '', @@ROWCOUNT);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @vchError = CONCAT(ERROR_PROCEDURE(), ': ', ERROR_MESSAGE());
        INSERT INTO @Result (bResult, vchMessage) VALUES (0, @vchError);
    END CATCH;

    SELECT * FROM @Result;
END
GO
```

En C#, el `Request` llevaría `IEnumerable<DTTest_TableItem> Items` y el repositorio usaría `GetWithListAsync` (un resultado) o `GetAllWithListAsync` (varios resultados).

 ## Ejemplo de Procedimiento Almacenado: Tipo Save

El siguiente ejemplo muestra un procedimiento de tipo `Save` para consultar información de la tabla `Usuarios`:

 ```sql
IF OBJECT_ID('spSaveUsers') IS NULL
    EXEC ('CREATE PROCEDURE spSaveUsers AS SET NOCOUNT ON;')
GO

ALTER PROCEDURE spSaveUsers
    (
    @iIdUsers INT,
    @iIdRoles INT,
    @vchName VARCHAR(50),
    @vchEmail VARCHAR(150),
    @vchPass VARCHAR(MAX),
    @vchNickName VARCHAR(50),
    @bStatus BIT
)
AS 
/*
** Creador:      Magdaleno Martinez
** Propósito:    Obtener los detalles de una cadena de conexión según el ID de usuario y su estado activo.
** Actualizado por: Alan
** Última fecha: 10/15/2025
*/
BEGIN
    -- Variables to manage transaction and error handling
    DECLARE @trancount INT = -1,
            @vchError VARCHAR(200) = '';

    -- Table variable for the response
    DECLARE @Result AS TABLE (
        bResult BIT DEFAULT(1),
        vchMessage VARCHAR(500) DEFAULT(''),
        iIdUsers INT DEFAULT(-1)
    );

    -- Disable row count messages
    SET NOCOUNT ON;

    BEGIN TRY
        SET XACT_ABORT ON;

        
        -- Checked that the registered email does not exist
        IF EXISTS (SELECT 1
            FROM dbo.Users U
            WHERE ( U.vchEmail = @vchEmail OR @vchNickName = @vchNickName)
                AND U.iIdUsers <> @iIdUsers
                AND U.bStatus = 1)
                BEGIN
                SET @vchError = 'The email O nickname already exists.'
                	
             GOTO _Fin

        END;


        -- Get current transaction count
        SET @trancount = @@TRANCOUNT;

        -- Save transaction state if already in transaction
        IF @trancount > 0
            SAVE TRANSACTION spSaveUsers;
        ELSE
            BEGIN TRANSACTION;


        -- Check if it's an insert or update operation
        IF @iIdUsers <= 0 
        BEGIN
        -- Insert new user
        INSERT INTO dbo.Users
            (
            iIdRoles, vchName,
            vchEmail, vchPass, bStatus, vchNickName
            )
        VALUES
            (
                3, @vchName,
                @vchEmail, HASHBYTES('SHA2_256', @vchPass), 1, @vchNickName
            );

        -- Retrieve the newly inserted identity
        SET @iIdUsers = SCOPE_IDENTITY();
    END
        ELSE
        BEGIN
        -- Update existing user
        UPDATE dbo.Users WITH(ROWLOCK)
            SET
                iIdRoles = @iIdRoles,
                vchName = @vchName,
                vchEmail = @vchEmail,
                bStatus = @bStatus,
                vchNickName =@vchNickName
            WHERE iIdUsers = @iIdUsers;
    END

        -- Commit transaction if it was started by this procedure
        IF @trancount = 0
            COMMIT TRANSACTION;
        
    END TRY
    BEGIN CATCH
        -- Capture error details
        SELECT @vchError = CONVERT(VARCHAR(200), CONCAT('spSaveUsers: ', ERROR_MESSAGE(), ' in ', ERROR_PROCEDURE(), ' at line ', ERROR_LINE()));
        PRINT CONCAT('spSaveUsers: ', ERROR_MESSAGE(), ' in ', ERROR_PROCEDURE(), ' at line ', ERROR_LINE());
        
        -- Rollback transaction if needed
        IF @trancount = 0
            ROLLBACK TRANSACTION;
        ELSE IF @trancount <> -1 AND XACT_STATE() <> -1
            ROLLBACK TRANSACTION spSaveUsers;
    END CATCH;

_Fin:
    -- Prepare response
    IF LEN(@vchError) > 0
    BEGIN
        INSERT INTO @Result
            (bResult, vchMessage)
        VALUES
            (0, @vchError);
    END
    ELSE
    BEGIN
        INSERT INTO @Result
            (bResult, vchMessage, iIdUsers)
        VALUES
            (1, '', @iIdUsers);
    END

    -- Return response
    SELECT *
    FROM @Result;

    -- Enable row count messages back
    SET NOCOUNT OFF;
END

GO

 ```

## EngramaCoreStandar ↔ procedimientos

Cada SP Engrama se invoca desde Infrastructure con `IDapperManagerHelper`:

| Forma del SP | Helper |
|--------------|--------|
| Sin lista, 1 fila (`Save` / detalle) | `GetAsync` |
| Sin lista, N filas | `GetAllAsync` |
| Con TVP (`@Lista DT* READONLY`), 1 fila | `GetWithListAsync` |
| Con TVP, N filas | `GetAllWithListAsync` |

Los campos `bResult` / `vchMessage` del SELECT final mapean a `DbResult`.
Guía: [8- Guia Consultas SP.md](8-%20Guia%20Consultas%20SP.md) · Paquete: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md).

