# Metodología Engrama para el Desarrollo de Software: Creación de Tablas en la Base de Datos

La metodología Engrama establece un conjunto de directrices claras y estructuradas para el diseño y creación de bases de datos, con un enfoque particular en la definición de tablas. Este documento detalla las convenciones para la creación de tablas en SQL Server, asegurando consistencia, claridad y escalabilidad en el desarrollo de software.

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