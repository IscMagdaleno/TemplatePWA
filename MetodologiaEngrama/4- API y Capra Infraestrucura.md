# Metodología Engrama para el Desarrollo de Software: Estructura de la API (Capa de Infraestructura)

La metodología Engrama define una arquitectura clara y estandarizada para el desarrollo de APIs, basada en los principios de **Clean Architecture**, pero adaptada para garantizar un proceso homogéneo y consistente en todas las aplicaciones desarrolladas bajo este enfoque. Este documento describe la estructura de la API, enfocándose exclusivamente en la capa de **Infraestructura** (`Infrastructure`), que es responsable de la interacción con la base de datos.

## Estructura General de la API

La API se organiza en dos capas principales de código base, denominadas **EngramaLevels**, y una capa de servicios que contiene los controladores con los endpoints. Estas capas, ordenadas desde la base de datos hasta el cliente, son:

1. **Infraestructura (Infrastructure)**: Gestiona la comunicación con la base de datos.
2. **Dominio (Domain)**: Encargada de la lógica de negocio de la aplicación.
3. **Servicios (Services)**: Contiene los controladores que exponen los endpoints para interactuar con el cliente.

Este documento se centra únicamente en la capa de **Infraestructura**, detallando su estructura, componentes y funcionamiento según las directrices de Engrama.

## Capa de Infraestructura

La capa de **Infraestructura** es responsable de todas las operaciones relacionadas con la base de datos, utilizando **procedimientos almacenados** y el micro ORM **Dapper** para consultas eficientes. La metodología Engrama estandariza este proceso mediante el uso del paquete NuGet **EngramaCoreStandard**, que proporciona una estructura simplificada para invocar procedimientos almacenados.

### Estructura de Carpetas
La capa de Infraestructura se organiza en tres carpetas principales:

1. **Interfaces**:  
   - Contiene las interfaces que definen los contratos para los repositorios.  
   - Cada interfaz se nombra con el prefijo `I` seguido del nombre del módulo, por ejemplo, `IUsuariosRepository.cs`.

2. **Repository**:  
   - Almacena las clases de repositorio que implementan las interfaces y gestionan las consultas a la base de datos.  
   - Cada clase se nombra según el módulo que representa, por ejemplo, `UsuariosRepository.cs`.

3. **Entities**:  
   - Contiene las clases que representan los procedimientos almacenados, con sus respectivas estructuras de entrada (`Request`) y salida (`Result`).  
   - Cada clase tiene el mismo nombre que el procedimiento almacenado al que corresponde.

### Convenciones para la Capa de Infraestructura

1. **Uso de Dapper y EngramaCoreStandard**:  
   - Todas las consultas a la base de datos se realizan mediante procedimientos almacenados, invocados a través de **Dapper**.  
   - El paquete **EngramaCoreStandard** proporciona métodos simplificados para interactuar con los procedimientos almacenados, como `GetAsync`, `GetAllAsync`, `GetWithListAsync` y `GetAllWithListAsync`.

2. **Repositorios**:  
   - Cada módulo tiene una clase de repositorio en la carpeta `Repository`, que implementa una interfaz correspondiente en la carpeta `Interfaces`.  
   - Ejemplo: Para el módulo de usuarios, se crea `UsuariosRepository.cs` (en `Repository`) e `IUsuariosRepository.cs` (en `Interfaces`).  
   - En el constructor de la clase de repositorio, se inyecta el servicio `IDapperManagerHelper` para facilitar las consultas a la base de datos.  
   - Ejemplo de implementación:

     ```csharp
     public class UsuariosRepository : IUsuariosRepository
     {
         private readonly IDapperManagerHelper _managerHelper;

         public UsuariosRepository(IDapperManagerHelper managerHelper)
         {
             _managerHelper = managerHelper;
         }
     }
     ```

3. **Entidades (Entities)**:  
   - Cada procedimiento almacenado tiene una clase en la carpeta `Entities` con el mismo nombre que el procedimiento, por ejemplo, `spGetTestTable.cs`.  
   - Esta clase contiene dos subclases:  
     - **Request**: Representa los parámetros de entrada del procedimiento almacenado. Implementa la interfaz `SpRequest` de `EngramaCoreStandard`, que requiere un atributo `StoredProcedure` con el nombre exacto del procedimiento.  
     - **Result**: Representa los datos devueltos por el procedimiento almacenado. Implementa la interfaz `DbResult`, que incluye los campos obligatorios `bResult` (indica si la consulta fue exitosa) y `vchMessage` (mensaje de error o nota para el cliente).  
   - Los atributos de `Request` y `Result` deben coincidir exactamente con los parámetros de entrada y salida del procedimiento almacenado.

   - Ejemplo de una entidad para el procedimiento `spGetTestTable`:

     ```csharp
     public class spGetTestTable
     {
         public class Request : SpRequest
         {
             public string StoredProcedure => "spGetTestTable";
             public int iIdTestTable { get; set; }
         }

         public class Result : DbResult
         {
             public bool bResult { get; set; }
             public string vchMessage { get; set; }
             public int iIdTestTable { get; set; }
             public string vchName { get; set; }
             public string vchEmail { get; set; }
             public DateTime? dtRegistered { get; set; }
         }
     }
     ```

4. **Manejo de Listas como Parámetros**:  
   - En casos donde el procedimiento almacenado requiere una lista de datos como parámetro (por ejemplo, un `DataTable`), se define un modelo en una carpeta separada con el prefijo `DT` seguido del nombre del tipo de datos, por ejemplo, `DTParameterType.cs`.  
   - Ejemplo de modelo para un `DataTable`:

     ```csharp
     public class DTParameterType
     {
         public int iParameter { get; set; }
         public string vchParameter { get; set; }
         public DateTime dtParameter { get; set; }
         public decimal mParameter { get; set; }
         public short smParameter { get; set; }
         public bool bParameter { get; set; }
     }
     ```

   - Ejemplo de una entidad que utiliza un `DataTable`:

     ```csharp
     public class spGetTestTableDataType
     {
         public class Request : SpRequest
         {
             public string StoredProcedure => "spGetTestTableDataType";
             public int iIdTestTable { get; set; }
             public string vchName { get; set; }
             public string vchEmail { get; set; }
             public DateTime? dtRegistered { get; set; }
             public IEnumerable<DTParameterType> Parameters { get; set; }
         }

         public class Result : DbResult
         {
             public bool bResult { get; set; }
             public string vchMessage { get; set; }
             public int iIdTestTable { get; set; }
             public string vchName { get; set; }
             public string vchEmail { get; set; }
             public DateTime? dtRegistered { get; set; }
         }
     }
     ```

5. **Métodos de Consulta**:  
   - Los métodos en las clases de repositorio invocan los procedimientos almacenados utilizando los métodos de `DapperManagerHelper`.  
   - Tipos de métodos disponibles según el tipo de consulta:  
     - `GetAsync`: Para procedimientos que retornan un solo registro.  
     - `GetAllAsync`: Para procedimientos que retornan una lista de registros (almacenada en un `IEnumerable`).  
     - `GetWithListAsync`: Para procedimientos que reciben una lista de datos (como un `DataTable`) y retornan un solo registro.  
     - `GetAllWithListAsync`: Para procedimientos que reciben una lista de datos y retornan una lista de registros.  
   - Ejemplo de un método para consultar `spGetTestTable`:

     ```csharp
     public async Task<IEnumerable<spGetTestTable.Result>> spGetTestTable(spGetTestTable.Request daoModel)
     {
         var respuesta = await _managerHelper.GetAllAsync<spGetTestTable.Result, spGetTestTable.Request>(daoModel, "");
         if (respuesta.Ok)
         {
             return respuesta.Data;
         }
         return new List<spGetTestTable.Result> { new() { bResult = false, vchMessage = respuesta.Msg } };
     }
     ```

### Beneficios de la Capa de Infraestructura
1. **Estandarización**: El uso de `EngramaCoreStandard` y las convenciones de nomenclatura garantizan consistencia en todas las aplicaciones.  
2. **Simplicidad**: La estructura de `Request` y `Result` simplifica la interacción con procedimientos almacenados.  
3. **Mantenibilidad**: La separación en interfaces, repositorios y entidades facilita la actualización y el mantenimiento del código.  
4. **Flexibilidad**: Los métodos de `DapperManagerHelper` cubren diferentes casos de uso, desde consultas simples hasta aquellas que manejan listas complejas.

### Recomendaciones
- Asegúrese de que los nombres de los procedimientos almacenados y sus parámetros sean idénticos a los definidos en las clases `Request` y `Result` para evitar errores.  
- Valide que los procedimientos almacenados estén optimizados para mejorar el rendimiento de las consultas.  
- Considere agregar documentación en el código (por ejemplo, comentarios XML) para mejorar la legibilidad y el mantenimiento.

## Conclusión
La capa de Infraestructura en la metodología Engrama proporciona una estructura robusta y estandarizada para interactuar con la base de datos, utilizando procedimientos almacenados y Dapper para consultas eficientes. La organización en carpetas (`Interfaces`, `Repository`, `Entities`) y las convenciones de nomenclatura aseguran que el código sea claro, mantenible y escalable, alineándose con los principios de Clean Architecture adaptados al enfoque Engrama.