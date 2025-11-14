# Manual Técnico

Este documento proporciona información técnica sobre la arquitectura, configuración, despliegue y mantenimiento del proyecto `API.backend.singula`.

## Resumen del proyecto
- Proyecto principal: `API.backend.singula` (API RESTful en ASP.NET Core, .NET 10).
- Biblioteca de dominio y servicios: `Singula.Core`.
- ORM: Entity Framework Core con `ApplicationDbContext`.
- Estructura: controladores en `API.backend.singula/Controllers`, lógica de negocio en `Singula.Core/Services`, repositorios en `Singula.Core/Repositories`.

## Requisitos
- .NET 10 SDK
- Base de datos (ajustar tipo según la cadena de conexión, comúnmente PostgreSQL en este proyecto)

## Estructura de carpetas
- `API.backend.singula/` — Proyecto API (controllers, configuración, `appsettings.json`).
- `Singula.Core/` — Lógica de negocio, entidades, DTOs, mapeos y repositorios.
- `docs/` — Documentación del proyecto.
- `postman/` — Colecciones Postman para pruebas.

## Entidades principales
- Revisa `Singula.Core/Core/Entities` y `ApplicationDbContext` para el listado completo de entidades: `Usuario`, `Personal`, `Solicitud`, `Reporte`, `ConfigSla`, `Area`, `Permiso`, `RolesSistema`, etc.

## Repositorios y servicios
- Repositorios en `Singula.Core/Repositories` (ej. `IUsuarioRepository`, `UsuarioRepository`, `EfRepository`).
- Servicios en `Singula.Core/Services` implementan la lógica de negocio y usan repositorios. Los DTOs están en `Singula.Core/Services/Dto`.

## Mapeos
- Se usa `AutoMapper` para mapear entre entidades y DTOs. Ver `Singula.Core/Mapping` para perfiles.

## Configuración
- `appsettings.json` contiene configuraciones. Use variables de entorno o secretos para datos sensibles.
- Revisar `Program.cs` para middlewares configurados: CORS, autenticación, Swagger, logging.

## Autenticación y autorización
- Si se usa JWT, la configuración está en `Program.cs` y `appsettings.json`. Proteja las claves y secretos.

## Despliegue
- Publicar: `dotnet publish -c Release -o ./publish`
- Ejecutar en servidor: `dotnet API.backend.singula.dll` o usar contenedor Docker.

## Pruebas
- Añadir un proyecto de pruebas (xUnit/NUnit) para servicios y controladores.

## Mantenimiento y recomendaciones
- Mantener credenciales fuera del repositorio.
- Añadir logging centralizado (Serilog) y tracing.
- Implementar middleware de manejo de errores y validación (ProblemDetails).
- Añadir Swagger/OpenAPI para documentación de endpoints.
- Revisar y limpiar entidades generadas por scaffolding (propiedades duplicadas y campos con inicializadores incompletos como `=;`).

## Problemas comunes por scaffolding
- El código generado puede contener duplicaciones en propiedades y colecciones (por ejemplo, propiedades repetidas con y sin inicializador `=;`).
- Revise cada clase en `Singula.Core/Core/Entities` y corrija duplicaciones, asegure que las colecciones estén inicializadas correctamente.

## Comandos útiles
- `dotnet restore`
- `dotnet build`
- `dotnet run --project API.backend.singula/API.backend.singula.csproj`
- `dotnet publish -c Release -o ./publish`

Fin del Manual Técnico.
