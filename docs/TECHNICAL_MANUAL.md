# Manual Técnico

## Resumen
Proyecto: `API.backend.singula` (.NET 10)
Core: `Singula.Core` (entidades, repositorios, servicios, DTOs)
ORM: Entity Framework Core + Npgsql (PostgreSQL)
Autenticación: JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`)
Swagger: `Swashbuckle.AspNetCore` (habilitado en Development)

## Estructura del proyecto
- `API.backend.singula`
  - `Program.cs` — configuración DI, DbContext, JWT, Swagger.
  - `Controllers/` — controladores API que consumen servicios y devuelven DTOs.
  - `appsettings.json` — `ConnectionStrings` y sección `Jwt`.
- `Singula.Core`
  - `Core/Entities` — entidades (scaffold).
  - `Infrastructure/Data/ApplicationDbContext.cs` — DbContext.
  - `Repositories/` — `IRepository<T>`, `EfRepository<T>`, repositorios específicos.
  - `Services/` — `I*Service` y `*Service` por entidad.
  - `Services/Dto/` — DTOs usados por las APIs.

## Principios de diseño
- Repositorio genérico + implementación EF.
- Servicios por entidad para encapsular lógica y mapeos.
- Controladores ligeros: usan servicios y retornan DTOs.
- Borrado lógico: si la entidad tiene `Estado` (string) o `EsActivo` (bool), `Delete` marca como inactivo; en caso contrario, borrado físico.

## Configuración importante
- `appsettings.json`:
  - `ConnectionStrings:DefaultConnection` — cadena PostgreSQL.
  - `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiresMinutes`.

Ejemplo mínimo:

```json
{
  "ConnectionStrings": { "DefaultConnection": "Host=localhost;Database=TTCBD;Username=postgres;Password=123456789" },
  "Jwt": { "Key":"super_secret_key_123!","Issuer":"singula","Audience":"singula_users","ExpiresMinutes":60 }
}
```

## Inyección de dependencias (resumen)
Registrado en `Program.cs`:
- `DbContext` (ApplicationDbContext)
- `IRepository<>` -> `EfRepository<>`
- Repositorios específicos (ej. `IUsuarioRepository`)
- Servicios por entidad (`I*Service` -> `*Service`)

## Endpoints principales
- Autenticación: `POST /api/usuarios/authenticate` ? `{ token }`
- CRUD por entidad en rutas `api/{entidad}` (retornan/reciben DTOs)

Ejemplos:
- `GET /api/areas`
- `POST /api/usuarios` (registro)
- `POST /api/usuarios/authenticate` (login)

Todos los endpoints (excepto autenticación y, opcionalmente, registro) requieren header:
```
Authorization: Bearer <token>
```

## DTOs
Cada entidad dispone de un DTO en `Singula.Core/Services/Dto`. Las relaciones se representan por ids.

## Operaciones con la base de datos
- No se crearon migrations automáticamente. Para generar migraciones y aplicar:
  - `dotnet ef migrations add Initial --project Singula.Core --startup-project API.backend.singula`
  - `dotnet ef database update --project Singula.Core --startup-project API.backend.singula`

## Recomendaciones
- Añadir validación (DataAnnotations / FluentValidation) a DTOs.
- Añadir middleware global de manejo de errores y logging.
- Considerar AutoMapper para evitar mapeos manuales.
- Proteger acciones por roles/claims si aplica.

---

Fecha: Generado automáticamente por el asistente de desarrollo.
