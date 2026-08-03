# Metodología Engrama: Capa de Dominio

La capa de **Dominio** (Domain) en la metodología Engrama es el núcleo donde se implementa la lógica de negocio, complementando la capa de Infraestructura, que se centra exclusivamente en la interacción con la base de datos. Esta capa se encarga de procesar todas las operaciones requeridas por el negocio que no estén implementadas en los procedimientos almacenados, los cuales deben contener la mayoría de las reglas de negocio. La capa de Dominio asegura que las aplicaciones desarrolladas bajo Engrama sean consistentes, modulares y escalables, siguiendo un enfoque basado en **Clean Architecture** adaptado a las necesidades de la metodología.

> **EngramaCoreStandar en Dominio:** `MapperHelper` (DTO ↔ `sp*.Request` / `Result` ↔ entidad Share) e `IResponseHelper` (`Validacion` → `Response<T>`). También se usan extensiones (`IsNull()`, `False()`, etc.). Catálogo: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md) §§ 4–5 y 9.

## Estructura de la Capa de Dominio

La capa de Dominio se organiza en tres carpetas principales, con la flexibilidad de agregar más según las necesidades del sistema:

1. **Core**:  
   - Contiene las clases base que implementan la lógica de negocio para cada módulo.  
   - Cada clase se nombra según el módulo al que corresponde, alineándose con la nomenclatura de la capa de Infraestructura. Por ejemplo, si en la capa de Infraestructura existe `UsuariosRepository`, en esta capa se llamará `UsuariosDomain`.

2. **Interfaces**:  
   - Almacena las interfaces que definen los contratos de las clases en `Core`.  
   - Cada interfaz lleva el prefijo `I` seguido del nombre de la clase, por ejemplo, `IUsuariosDomain`.

3. **Servicios**:  
   - Contiene servicios adicionales que encapsulan reglas de negocio específicas según los requisitos del módulo.  
   - Estos servicios se crean según sea necesario y pueden variar en función del proyecto.

Si el sistema requiere más carpetas (por ejemplo, para validaciones específicas o utilidades), estas se pueden agregar según las necesidades del proyecto, manteniendo la modularidad.

## Configuración de las Clases en la Capa de Dominio

Cada clase en la carpeta `Core` debe configurarse siguiendo las directrices de Engrama, utilizando inyección de dependencias para integrar las siguientes dependencias obligatorias:

1. **Repositorio de la Capa de Infraestructura**:  
   - Una interfaz del repositorio correspondiente al módulo (por ejemplo, `ITestRepository` para el módulo `Test`).  
   - Permite invocar los métodos que consultan la base de datos desde la capa de Infraestructura.

2. **MapperHelper**:  
   - Una herramienta proporcionada por el paquete `EngramaCoreStandar`, basada en un mapeador como AutoMapper, que facilita la conversión entre modelos (por ejemplo, de `DtoModel` a `Request`).

3. **IResponseHelper**:  
   - Una interfaz que ayuda a validar y estandarizar las respuestas de la capa de Infraestructura, devolviendo un objeto `Response<T>` que homogeneiza las respuestas.

### Ejemplo de Configuración de una Clase

El siguiente ejemplo muestra la configuración inicial de una clase en la capa de Dominio para el módulo `Test`:

```csharp
public class TestDomain : ITestDomain
{
    private readonly ITestRepository testRepository;
    private readonly MapperHelper mapperHelper;
    private readonly IResponseHelper responseHelper;

    /// <summary>
    /// Inicializa las dependencias mediante inyección en el constructor.
    /// </summary>
    public TestDomain(ITestRepository testRepository, MapperHelper mapperHelper, IResponseHelper responseHelper)
    {
        this.testRepository = testRepository;
        this.mapperHelper = mapperHelper;
        this.responseHelper = responseHelper;
    }
}
```

#### Explicación
- **Inyección de Dependencias**: Las dependencias (`ITestRepository`, `MapperHelper`, `IResponseHelper`) se inyectan a través del constructor, promoviendo un diseño modular y testable.
- **Nomenclatura**: La clase `TestDomain` y su interfaz `ITestDomain` siguen la convención de nombres alineada con el módulo `Test` en la capa de Infraestructura.

## Métodos en la Capa de Dominio

Los métodos en la capa de Dominio se diseñan para procesar la lógica de negocio y comunicarse con la capa de Infraestructura. Cada método debe:

1. **Nombrarse según el procedimiento almacenado**: El nombre del método elimina el prefijo `sp` del procedimiento correspondiente. Por ejemplo, para `spSaveUsuarios`, el método se llamará `SaveUsuarios`.
2. **Recibir un DtoModel como parámetro**: El parámetro de entrada es una clase `DtoModel` definida en el proyecto `Share`, específica para el proceso (por ejemplo, `DtoSaveUsuarios`).
3. **Devolver un objeto `Response<T>`**: Todas las respuestas se encapsulan en un objeto `Response<T>` para estandarizar el formato, incluyendo:
   - `Data`: Los datos devueltos (tipo `T`).
   - `IsSuccess`: Indica si la operación fue exitosa (`true`) o no (`false`).
   - `Message`: Mensaje descriptivo en caso de error o nota adicional.

### Definición de la Clase `Response<T>`

```csharp
public class Response<T>
{
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static Response<T> BadResult(string message, T data)
    {
        return new Response<T>
        {
            IsSuccess = false,
            Message = message,
            Data = data
        };
    }
}
```

### Ejemplo de Método: Consulta de Lista de Datos

El siguiente método consulta una lista de datos usando el procedimiento almacenado `spGetTestTable`:

```csharp
public async Task<Response<IEnumerable<TestTable>>> GetTestTable(DtoTestTable daoModel)
{
    try
    {
        // Mapear DtoModel a Request
        var model = mapperHelper.Get<DtoTestTable, spGetTestTable.Request>(daoModel);

        // Invocar el repositorio
        var result = await testRepository.spGetTestTable(model);

        // Validar y mapear la respuesta
        return responseHelper.Validacion<spGetTestTable.Result, TestTable>(result);
    }
    catch (Exception ex)
    {
        return Response<IEnumerable<TestTable>>.BadResult(ex.Message, new List<TestTable>());
    }
}
```

#### Explicación
- **Mapeo**: `mapperHelper` convierte el modelo `DtoTestTable` (del proyecto `Share`) en `spGetTestTable.Request` para la capa de Infraestructura.
- **Consulta**: Se invoca el método `spGetTestTable` del repositorio, que ejecuta el procedimiento almacenado.
- **Validación**: `responseHelper` valida la respuesta y la mapea al modelo `TestTable`, devolviendo un `Response<IEnumerable<TestTable>>`.
- **Manejo de Errores**: En caso de excepción, se devuelve un `Response` con `IsSuccess = false` y el mensaje de error.

### Ejemplo de Método: Operación de Escritura

El siguiente método gestiona la creación o actualización de un registro usando el procedimiento almacenado `spSaveTestTable`:

```csharp
public async Task<Response<TestTable>> SaveTestTable(DtoSaveTestTable DtoModel)
{
    try
    {
        // Mapear DtoModel a Request
        var model = mapperHelper.Get<DtoSaveTestTable, spSaveTestTable.Request>(DtoModel);

        // Invocar el repositorio
        var result = await testRepository.spSaveTestTable(model);

        // Validar y mapear la respuesta
        var validation = responseHelper.Validacion<spSaveTestTable.Result, TestTable>(result);

        // Actualizar el DtoModel con el ID generado (si es un INSERT)
        if (validation.IsSuccess)
        {
            DtoModel.iIdTestTable = validation.Data.iIdTestTable;
            validation.Data = mapperHelper.Get<DtoSaveTestTable, TestTable>(DtoModel);
        }

        return validation;
    }
    catch (Exception ex)
    {
        return Response<TestTable>.BadResult(ex.Message, new TestTable());
    }
}
```

#### Explicación
- **Mapeo**: Convierte `DtoSaveTestTable` en `spSaveTestTable.Request` para la capa de Infraestructura.
- **Consulta**: Invoca el método `spSaveTestTable` del repositorio.
- **Validación**: `responseHelper` valida la respuesta y la mapea a `TestTable`.
- **Lógica Adicional**: Si la operación es exitosa (`IsSuccess = true`), actualiza el `DtoModel` con el ID generado y mapea los datos de vuelta al modelo `TestTable`.
- **Manejo de Errores**: Devuelve un `Response` con `IsSuccess = false` en caso de excepción.

## Proceso de Desarrollo en la Capa de Dominio

1. **Mapeo del Modelo**:  
   - Usar `mapperHelper` para convertir el `DtoModel` (del proyecto `Share`) en el modelo `Request` de la entidad correspondiente al procedimiento almacenado.

2. **Invocación del Repositorio**:  
   - Llamar al método del repositorio en la capa de Infraestructura que corresponde al procedimiento almacenado, pasando el modelo `Request`.

3. **Validación de la Respuesta**:  
   - Utilizar `responseHelper` para validar la respuesta del repositorio y mapearla al modelo esperado (`TestTable` en los ejemplos).

4. **Lógica de Negocio Adicional**:  
   - Si es necesario, agregar lógica de negocio después de validar que `IsSuccess = true`. Por ejemplo, actualizar campos en el `DtoModel` o realizar cálculos adicionales.

5. **Respuesta Estandarizada**:  
   - Siempre devolver un objeto `Response<T>`, con `IsSuccess` y `Message` actualizados según el resultado de la operación.

## Ejemplo: SP con lista (DataType / TVP)

Cuando el DTO trae una colección que debe llegar a SQL como Table-Valued Parameter:

1. **No** uses `mapperHelper.Get` del DTO completo si Share `DT*` y Infrastructure `DT*` son tipos distintos (AutoMapper falla en `Parameters`).
2. Construye `sp*.Request` con escalares + **asigna explícitamente** la lista TVP mapeada a `Infrastructure.Entity.DT*`.
3. El repositorio debe usar `GetWithListAsync` / `GetAllWithListAsync`.

```csharp
public async Task<Response<IEnumerable<Test_Table>>> GetTestTableDataType(DtoGetTestTableDataType postModel)
{
    // Do NOT mapperHelper.Get the full DTO: Share.DT* ≠ Infrastructure.DT* → AutoMapper fails on Parameters.
    var model = new spGetTestTableDataType.Request
    {
        iIdTest_Table = postModel.iIdTest_Table,
        vchName = postModel.vchName,
        vchEmail = postModel.vchEmail,
        dtRegistered = postModel.dtRegistered,
        Parameters = (postModel.Parameters ?? Enumerable.Empty<Share.DtoModels.TestModule.DTParameterType>())
            .Select(p => new Infrastructure.Entity.DTParameterType
            {
                iParameter = p.iParameter,
                vchParameter = p.vchParameter,
                dtParameter = p.dtParameter,
                mParametr = p.mParametr,
                smParameter = p.smParameter,
                bParameter = p.bParameter
            })
            .ToList()
    };

    var result = await testRepository.spGetTestTableDataType(model);
    return responseHelper.Validacion<spGetTestTableDataType.Result, Test_Table>(result);
}
```

Referencias: `BaseDatos/ScritpCompleto.sql` (`DTParameterType`, `spGetTestTableDataType`), docs `1-`, `2-` y `4-`.

## EngramaCoreStandar en Dominio

| Pieza | API del paquete | Uso típico |
|-------|-----------------|------------|
| `MapperHelper` | `Get<TIn,TOut>`, `GetEnumerable`, `Map` | DTO → `sp*.Request`; `Result` → Object Share |
| `IResponseHelper` | `Validacion<TResult,TOut>(…)`, `ValidacionSinMapper` | Lista o fila `DbResult` → `Response<T>` |
| `Response<T>` | `IsSuccess`, `Message`, `Data`, `BadResult` | Contrato hacia Controllers |
| Extensiones | `IsNull()`, `False()`, `IsEmpty()`, … | Guards idiomáticos Engrama |

```csharp
using EngramaCoreStandar.Extensions;

if (postModel.IsNull().False())
{
    // postModel no es null
}

return responseHelper.Validacion<spSaveTest_Table.Result, Test_Table>(result);
return responseHelper.Validacion<spGetTestTable.Result, Test_Table>(resultList);
```

`MapperHelper` y `IResponseHelper` se registran con `AddEngramaDependenciesAPI()`. Detalle: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md) §§ 4–5 y 9.

## Recomendaciones
- **Modularidad**: Mantenga las clases en `Core` enfocadas en un solo módulo para facilitar el mantenimiento.
- **Lógica de Negocio**: Limite la lógica en la capa de Dominio a operaciones que no puedan implementarse en los procedimientos almacenados, respetando la directriz de Engrama de priorizar la base de datos para reglas de negocio.
- **Validaciones**: Considere agregar validaciones adicionales en los métodos (por ejemplo, verificar que el `DtoModel` no sea nulo antes del mapeo).
- **Pruebas**: Implemente pruebas unitarias para los métodos de la capa de Dominio, asegurando que las respuestas sean correctas en diferentes escenarios.
- **Documentación**: Agregue comentarios XML a todos los métodos para mejorar la mantenibilidad y la comprensión del código.

## Conclusión
La capa de Dominio en la metodología Engrama es el componente clave para implementar la lógica de negocio, integrando las consultas a la base de datos (a través de la capa de Infraestructura) con las necesidades del cliente. La estructura modular en carpetas (`Core`, `Interfaces`, `Servicios`), la inyección de dependencias (`ITestRepository`, `MapperHelper`, `IResponseHelper` de **EngramaCoreStandar**) y el uso del objeto `Response<T>` garantizan homogeneidad, escalabilidad y facilidad de mantenimiento.