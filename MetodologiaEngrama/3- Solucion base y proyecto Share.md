# Metodología Engrama para el Desarrollo de Software: Estructura de Proyectos

La metodología Engrama proporciona un marco estructurado y modular para el desarrollo de soluciones de software, con un enfoque en la organización, escalabilidad y reutilización de componentes. Este documento detalla la estructura inicial de los proyectos dentro de la metodología, centrándose en la solución web, su división en proyectos y la descripción específica del proyecto compartido (`Share`).

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
- **NuGet EngramaCoreStandard**:  
   - Este paquete es la base de la metodología Engrama, proporcionando métodos y funcionalidades estándar que garantizan el orden y la consistencia en el desarrollo.  
   - Se incluye en el proyecto `Share` para que tanto la API como el cliente hereden estas funcionalidades, promoviendo la estandarización en toda la solución.

### Estructura de Carpetas
El proyecto `Share` se organiza en carpetas para mantener una estructura clara y modular:

1. **Carpeta `Objetos`**:  
   - Contiene submódulos organizados por área o módulo funcional.  
   - Cada submódulo se nombra con el formato `[NombreMódulo]Modulo`. Ejemplo: `UsuariosModulo`.  
   - Dentro de cada carpeta se almacenan los modelos de datos específicos del módulo correspondiente, representando las entidades utilizadas en esa área.  
   - Ejemplo: En `UsuariosModulo`, se incluirían modelos como `Usuario`, `Rol`, etc.

2. **Carpeta `DtoClass`**:  
   - Contiene clases destinadas a interactuar con los midday:  
     - Estas clases siguen la nomenclatura `[NombreMódulo]Modulo`. Ejemplo: `UsuariosModulo`.  
     - Cada clase dentro de esta carpeta corresponde directamente a un endpoint de la API, con el mismo nombre que el endpoint al que se dirige.  
     - Ejemplo: Si existe una clase llamada `DtoSaveUsuarios`, el endpoint asociado en la API debe llamarse `[HttpPost("DtoSaveUsuarios")]`.  
     - Esto asegura una correspondencia directa entre las clases y los endpoints, facilitando la trazabilidad y el mantenimiento.  
     - Cada endpoint tiene una clase asociada en `DtoClass`, garantizando una relación uno a uno.

### Convenciones de Nomenclatura
- **Modelos**: Los nombres de los modelos en la carpeta `Objetos` deben reflejar claramente su propósito, utilizando sustantivos en singular (por ejemplo, `Usuario`, `Producto`).  
- **Clases PostClass**: Los nombres de las clases deben coincidir exactamente con los nombres de los endpoints, asegurando consistencia y claridad.  
- **Postfijos**: El uso del postfijo `Modulo` en los nombres de carpetas y clases dentro de `PostClass` garantiza una identificación clara de su propósito dentro del módulo.

## Ejemplo de Implementación

A continuación, se muestra un ejemplo de cómo se organizaría el proyecto `Share` para el módulo `Usuarios`:

- **Carpeta `Objetos/UsuariosModulo`**:  
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

- **Carpeta `DtoClass/UsuariosModulo`**:  
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

## Beneficios de la Estructura
1. **Reutilización**: Los modelos y clases compartidas reducen la duplicación de código entre la API y el cliente.  
2. **Claridad**: La organización por módulos y la nomenclatura estandarizada facilitan la comprensión y el mantenimiento del código.  
3. **Escalabilidad**: La estructura modular permite agregar nuevos módulos sin afectar los existentes.  
4. **Consistencia**: La correspondencia entre clases y endpoints simplifica el desarrollo y la integración.

## Conclusión
La metodología Engrama, a través de la estructura del proyecto `Share` y la división en módulos, proporciona una base sólida para desarrollar soluciones web organizadas y escalables. La integración del paquete `EngramaCoreStandard` y las convenciones de nomenclatura aseguran que los proyectos sean coherentes, fáciles de mantener y alineados con los estándares de la metodología.