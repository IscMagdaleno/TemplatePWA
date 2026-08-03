# Metodología Engrama: Capa de Servicio (Controladores)

La capa de **Servicio** en la metodología Engrama corresponde a los controladores de la API, los cuales exponen los endpoints que la aplicación cliente (por ejemplo, el cliente Blazor PWA) consume. Esta capa actúa como la interfaz principal entre el cliente y la lógica de negocio implementada en la capa de Dominio, siguiendo un enfoque basado en **Clean Architecture** adaptado a las necesidades de Engrama. Este documento detalla la estructura, convenciones y ejemplos para los controladores, asegurando homogeneidad y facilidad de integración con las capas inferiores.

> **EngramaCoreStandar:** los controladores no usan Dapper directamente. Devuelven `Response<T>` (`IsSuccess`, `Message`, `Data`) producido por Dominio vía `IResponseHelper`. El cliente Blazor deserializa ese envelope con `IHttpService` + `IValidaServicioService`. Ver [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md) §§ 4 y 6.

## Estructura de la Capa de Servicio

Los controladores en la capa de Servicio se organizan para reflejar los módulos definidos en las capas de Dominio e Infraestructura, garantizando consistencia en la nomenclatura y funcionalidad. Cada controlador se nombra según el módulo al que corresponde, alineándose con las clases de las capas inferiores. Por ejemplo:

- Si en la capa de Infraestructura existe `UsuariosRepository` y en la capa de Dominio `UsuariosDomain`, el controlador se llamará `UsuariosController`.

### Configuración de los Controladores

Cada controlador hereda de `ControllerBase` y utiliza inyección de dependencias para integrar la interfaz de la capa de Dominio correspondiente al módulo. Los controladores están decorados con los atributos `[ApiController]` y `[Route("api/[controller]")]` para seguir las convenciones de ASP.NET Core.

#### Ejemplo de Configuración de un Controlador

El siguiente ejemplo muestra la configuración de un controlador para el módulo `Test`:

```csharp
/// <summary>
/// Controlador para el módulo Test, que demuestra la integración con la metodología Engrama.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ITestDomain testDomain;

    /// <summary>
    /// Inicializa el controlador con la dependencia de la capa de Dominio.
    /// </summary>
    /// <param name="testDomain">Interfaz de la capa de Dominio para el módulo Test.</param>
    public TestController(ITestDomain testDomain)
    {
        this.testDomain = testDomain;
    }
}
```

#### Explicación
- **Herencia**: El controlador hereda de `ControllerBase`, proporcionando las funcionalidades básicas para manejar solicitudes HTTP en ASP.NET Core.
- **Inyección de Dependencias**: Se inyecta la interfaz de la capa de Dominio (`ITestDomain`) a través del constructor, permitiendo el acceso a los métodos de lógica de negocio.
- **Atributos**:
  - `[ApiController]`: Habilita características de ASP.NET Core como la validación automática de modelos.
  - `[Route("api/[controller]")]`: Define la ruta base del controlador (por ejemplo, `/api/Test`).
- **Documentación**: Los comentarios XML describen el propósito del controlador, facilitando su integración con herramientas como Swagger.

## Convenciones para los Endpoints

Los endpoints en los controladores son métodos que exponen las funcionalidades de la API al cliente. Cada endpoint sigue estas directrices:

1. **Nomenclatura**:
   - El nombre del método coincide con el nombre del `DtoModel` definido en el proyecto `Share`, eliminando ambigüedades. Por ejemplo, para el modelo `DtoTestTable`, el endpoint se llama `DtoTestTable`.
   - El atributo de ruta HTTP incluye el nombre del método (por ejemplo, `[HttpPost("DtoTestTable")]`).

2. **Parámetros**:
   - Los endpoints reciben un `DtoModel` como parámetro, enviado desde el cliente mediante el cuerpo de la solicitud (`[FromBody]`).

3. **Respuesta**:
   - Los métodos devuelven un `IActionResult`, retornando `Ok` si la operación es exitosa (`IsSuccess = true`) o `BadRequest` si falla (`IsSuccess = false`), basándose en el objeto `Response<T>` de la capa de Dominio.

4. **Documentación**:
   - Cada método debe incluir comentarios XML que describan su propósito, parámetros y valor de retorno, facilitando la integración con Swagger y la comprensión por parte de los desarrolladores.

### Ejemplo de Endpoint: Consulta de Datos

El siguiente ejemplo muestra un endpoint para consultar datos usando el método `GetTestTable` de la capa de Dominio:

```csharp
/// <summary>
/// Consulta una lista de datos de la tabla TestTable utilizando la metodología Engrama.
/// </summary>
/// <param name="DtoModel">Modelo de entrada con los parámetros para la consulta.</param>
/// <returns>Respuesta con los datos consultados o un mensaje de error.</returns>
[HttpPost("PostTestTable")]
public async Task<IActionResult> PostTestTable([FromBody] DtoTestTable DtoModel)
{
    var result = await testDomain.GetTestTable(DtoModel);
    if (result.IsSuccess)
    {
        return Ok(result);
    }
    return BadRequest(result);
}
```

#### Explicación
- **Ruta**: `[HttpPost("PostTestTable")]` define el endpoint como `/api/Test/PostTestTable`.
- **Parámetro**: `DtoTestTable` es el modelo recibido desde el cliente, definido en el proyecto `Share`.
- **Lógica**: Invoca el método `GetTestTable` de la capa de Dominio, que procesa la lógica de negocio y retorna un `Response<IEnumerable<TestTable>>`.
- **Respuesta**: Devuelve `Ok(result)` si `IsSuccess = true`, o `BadRequest(result)` si `IsSuccess = false`.
- **Documentación**: Los comentarios XML detallan el propósito, el parámetro `DtoModel` y el valor de retorno, compatibles con Swagger.

### Ejemplo de Endpoint: Operación de Escritura

El siguiente ejemplo muestra un endpoint para guardar o actualizar datos usando el método `SaveTestTable` de la capa de Dominio:

```csharp
/// <summary>
/// Guarda o actualiza un registro en la tabla TestTable utilizando la metodología Engrama.
/// </summary>
/// <param name="DtoModel">Modelo de entrada con los datos del registro.</param>
/// <returns>Respuesta con el registro procesado o un mensaje de error.</returns>
[HttpPost("PostSaveTestTable")]
public async Task<IActionResult> PostSaveTestTable([FromBody] DtoSaveTestTable DtoModel)
{
    var result = await testDomain.SaveTestTable(DtoModel);
    if (result.IsSuccess)
    {
        return Ok(result);
    }
    return BadRequest(result);
}
```

#### Explicación
- **Ruta**: `[HttpPost("PostSaveTestTable")]` define el endpoint como `/api/Test/PostSaveTestTable`.
- **Parámetro**: `DtoSaveTestTable` contiene los datos para crear o actualizar un registro.
- **Lógica**: Invoca `SaveTestTable` de la capa de Dominio, que maneja la lógica de negocio y retorna un `Response<TestTable>`.
- **Respuesta**: Similar al endpoint anterior, retorna `Ok` o `BadRequest` según el valor de `IsSuccess`.
- **Documentación**: Incluye comentarios XML para claridad y soporte en Swagger.

## Proceso de Desarrollo en la Capa de Servicio

1. **Configuración del Controlador**:
   - Crear una clase que herede de `ControllerBase` y use los atributos `[ApiController]` y `[Route("api/[controller]")]`.
   - Inyectar la interfaz de la capa de Dominio correspondiente en el constructor.

2. **Definición de Endpoints**:
   - Nombrar el método según el `DtoModel` correspondiente (por ejemplo, `DtoTestTable`).
   - Usar el atributo HTTP adecuado (`[HttpPost]`, `[HttpGet]`, etc.) y definir la ruta específica.
   - Recibir el `DtoModel` desde el cuerpo de la solicitud con `[FromBody]`.

3. **Invocación de la Capa de Dominio**:
   - Llamar al método correspondiente de la capa de Dominio, pasando el `DtoModel`.
   - La capa de Dominio maneja toda la lógica de negocio, incluyendo mapeo, consultas a la base de datos y validaciones.

4. **Procesamiento de la Respuesta**:
   - Evaluar el campo `IsSuccess` del objeto `Response<T>` retornado por la capa de Dominio.
   - Devolver `Ok(result)` para respuestas exitosas o `BadRequest(result)` para errores.

5. **Documentación**:
   - Incluir comentarios XML detallados para cada método, describiendo su propósito, parámetros y valor de retorno.

## EngramaCoreStandar en Controllers

Los controladores **no** referencian Dapper ni `MapperHelper`. Solo consumen:

| Tipo | Paquete | Rol |
|------|---------|-----|
| `Response<T>` | `EngramaCoreStandar.Results` | Envelope uniforme (`IsSuccess` / `Message` / `Data`) |
| Dominio | — | Produce el `Response<T>` vía `IResponseHelper` |

El cliente Blazor lee el mismo envelope con `IHttpService` + `IValidaServicioService.ValidadionServicio`.
Si el body llega con HTTP 400, `HttpService` aún puede deserializar `Response<T>` con `IsSuccess = false` (patrón Engrama).

Registro API: `builder.Services.AddEngramaDependenciesAPI();` — ver [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md) § 1.

## Recomendaciones

- **Minimización de Lógica**: Mantenga los controladores lo más ligeros posible, delegando toda la lógica de negocio a la capa de Dominio.
- **Validaciones**: Considere agregar validaciones de entrada (por ejemplo, verificar que el `DtoModel` no sea nulo) antes de invocar la capa de Dominio.
- **Seguridad**: Implemente autenticación y autorización (por ejemplo, con `[Authorize]`) en los endpoints según los requisitos del sistema.
- **Documentación**: Asegúrese de que todos los endpoints tengan comentarios XML completos para facilitar la integración con Swagger y la documentación automática.
- **Pruebas**: Desarrolle pruebas de integración para los endpoints, verificando respuestas `Ok` y `BadRequest` en diferentes escenarios.

## Conclusión

La capa de Servicio en la metodología Engrama, representada por los controladores, es la interfaz principal para la interacción entre la aplicación cliente y la lógica de negocio. Al seguir una nomenclatura homogénea y el envelope `Response<T>` de **EngramaCoreStandar**, esta capa garantiza una integración fluida con Dominio y el cliente Blazor.