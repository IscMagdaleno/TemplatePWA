# Metodología Engrama para el Desarrollo de Software: Estructura de Proyectos

La metodología Engrama proporciona un marco estructurado y modular para el desarrollo de soluciones de software, con un enfoque en la organización, escalabilidad y reutilización de componentes. Este documento detalla la estructura inicial de los proyectos dentro de la metodología, centrándose en la solución web, su división en proyectos y la descripción específica del proyecto compartido (`Share`).

> **EngramaCoreStandar:** es el cuarto “proyecto” implícito de la solución. En TemplatePWA se agrega a `TemplateBlazorPWA.sln` y se referencia por `ProjectReference` desde Share (y API/PWA). Referencia completa de APIs: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md).

## Estructura de la Solución Web

La solución web se compone de tres proyectos principales, diseñados para trabajar de manera conjunta y garantizar modularidad y claridad en el desarrollo:

1. **API**:  
   - Proyecto encargado de exponer los servicios y endpoints que gestionan la lógica de negocio y la interacción con la base de datos.  
   - Implementa una arquitectura RESTful para facilitar la comunicación con el cliente.

2. **Cliente Blazor (PWA)**:  
   - Proyecto frontend basado en Blazor, configurado como una Progressive Web App (PWA), que ofrece una experiencia de usuario fluida, responsiva y accesible sin conexión.  
   - Consume los endpoints de la API para interactuar con los datos.

3. **Share**:  
   - Proyecto tipo librería de clases que contiene los modelos y funcionalidades compartidas entre la API y el cliente PWA.  
   - Promueve la reutilización de código y asegura consistencia en la estructura de datos.

## Organización por Módulos/Áreas

La solución se divide en **módulos**, cada una representando un conjunto de funcionalidades relacionadas con un propósito específico. Por ejemplo:  
- **Módulo Usuarios**: Gestiona todas las operaciones relacionadas con usuarios, incluyendo consulta, creación, actualización y eliminación (CRUD).  
- Cada módulo agrupa las funcionalidades que comparten un mismo objetivo, facilitando la organización y el mantenimiento del código.

## Proyecto Share: Descripción y Estructura

El proyecto `Share` es una librería de clases que centraliza los modelos y funcionalidades compartidas entre la API y el cliente PWA, asegurando consistencia y evitando duplicidad de código. A continuación, se detalla su estructura y propósito:

### Dependencias
- **EngramaCoreStandar** (código local o NuGet):  
   - Base de la metodología: Dapper helpers, `Response<T>`, `MapperHelper`, HTTP client, extensiones.  
   - En este template, Share referencia:
     ```xml
     <ProjectReference Include="..\..\EngramaCoreStandar\EngramaCoreStandar\EngramaCoreStandar.csproj" />
     ```
   - API registra `AddEngramaDependenciesAPI()`; PWA registra `AddEngramaDependenciesBlazor()` + `HttpClient`.  
   - Detalle de cada servicio: [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md).

### Estructura de Carpetas
El proyecto `Share` se organiza en carpetas para mantener una estructura clara y modular. Los nombres de carpeta en el template son **`Objects`** y **`DtoModels`** (alineados con el código):

1. **Carpeta `Objects`**:  
   - Contiene submódulos organizados por área o módulo funcional.  
   - Cada submódulo se nombra con el formato `[Nombre]Module`. Ejemplo: `UsuariosModule`, `TestModule`.  
   - Dentro de cada carpeta se almacenan los modelos de datos específicos del módulo correspondiente, representando las entidades utilizadas en esa área.  
   - Ejemplo: En `UsuariosModule`, se incluirían modelos como `Usuario`, `Rol`, etc.

2. **Carpeta `DtoModels`**:  
   - Contiene clases destinadas a interactuar con los endpoints:  
     - Estas carpetas siguen la nomenclatura `[Nombre]Module`. Ejemplo: `UsuariosModule`.  
     - Cada clase dentro de esta carpeta corresponde directamente a un endpoint de la API, con el mismo nombre que el endpoint al que se dirige.  
     - Ejemplo: Si existe una clase llamada `DtoSaveUsuarios`, el endpoint asociado en la API debe llamarse `[HttpPost("DtoSaveUsuarios")]`.  
     - Esto asegura una correspondencia directa entre las clases y los endpoints, facilitando la trazabilidad y el mantenimiento.  
     - Cada endpoint tiene una clase asociada en `DtoModels`, garantizando una relación uno a uno.

### Convenciones de Nomenclatura
- **Modelos**: Los nombres de los modelos en la carpeta `Objects` deben reflejar claramente su propósito, utilizando sustantivos en singular (por ejemplo, `Usuario`, `Producto`).  
- **Clases DTO**: Los nombres de las clases deben coincidir exactamente con los nombres de los endpoints, asegurando consistencia y claridad.  
- **Módulos**: El postfijo `Module` en los nombres de carpetas garantiza una identificación clara del módulo (ejemplo: `TestModule`).

## Ejemplo de Implementación

A continuación, se muestra un ejemplo de cómo se organizaría el proyecto `Share` para el módulo `Usuarios`:

- **Carpeta `Objects/UsuariosModule`**:  
  - Contiene modelos como:  
    ```csharp
    public class Usuario
    {
        public int iIdUsuario { get; set; }
        public string vchNombre { get; set; }
        public string vchCorreo { get; set; }
        public bool bActivo { get; set; }
    }
    ```

- **Carpeta `DtoModels/UsuariosModule`**:  
  - Contiene clases como:  
    ```csharp
    public class DtoSaveUsuarios
    {
        public int iIdUsuario { get; set; }
        public string vchNombre { get; set; }
        public string vchCorreo { get; set; }
        public bool bActivo { get; set; }
    }
    ```
  - Esta clase se corresponde con el endpoint `[HttpPost("DtoSaveUsuarios")]` en la API.

### Tipos compartidos que vienen de EngramaCoreStandar

Share (y por transitividad API/PWA) consume del paquete tipados comunes sin redefinirlos:

| Tipo | Uso entre capas |
|------|-----------------|
| `Response<T>` | Dominio → Controller → `IHttpService` |
| `SeverityMessage` | Cliente tras `ValidadionServicio` |
| Extensiones | Útiles en Share / Main* si se necesitan |

Los modelos de negocio del módulo viven en Share (`Objects/`, `DtoModels/`); los contratos SQL (`sp*`, `DT*`) viven en API Infrastructure.

## Beneficios de la Estructura
1. **Reutilización**: Los modelos y clases compartidas reducen la duplicación de código entre la API y el cliente.
2. **Claridad**: La organización por módulos y la nomenclatura estandarizada facilitan la comprensión y el mantenimiento del código.
3. **Escalabilidad**: La estructura modular permite agregar nuevos módulos sin afectar los existentes.
4. **Consistencia**: La correspondencia entre clases y endpoints simplifica el desarrollo y la integración.

## Conclusión
La metodología Engrama, a través del proyecto `Share` y **EngramaCoreStandar**, proporciona una base sólida para soluciones web organizadas y escalables. Ver [0- EngramaCoreStandar.md](0-%20EngramaCoreStandar.md).