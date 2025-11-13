# Manual de Usuario

## Requisitos
- .NET 10 SDK instalado
- Acceso a PostgreSQL con la cadena de conexión configurada en `appsettings.json`

## Iniciar la API
1. Configura `appsettings.json` (connection string y JWT).
2. Desde la carpeta del proyecto API:
   - `dotnet build`
   - `dotnet run`
3. En desarrollo, abrir `https://localhost:{port}/swagger` para explorar la API.

## Flujo de uso
1. Crear usuario (opcional si ya existen usuarios):
   - `POST /api/usuarios` con JSON:
     ```json
     {
       "username":"admin1",
       "correo":"admin@example.com",
       "password":"P@ssw0rd",
       "idRolSistema":1,
       "idEstadoUsuario":1
     }
     ```
2. Autenticar:
   - `POST /api/usuarios/authenticate` con `{ "username":"admin1", "password":"P@ssw0rd" }`
   - Copiar el `token` devuelto.
3. Probar endpoints protegidos:
   - En headers: `Authorization: Bearer <token>`
   - Ejemplos:
     - `GET /api/areas`
     - `POST /api/areas` con `{ "nombreArea":"Soporte","descripcion":"Area de soporte" }`

## Borrado lógico
- Para entidades con `Estado` o `EsActivo`, el `DELETE` marca la entidad como inactiva en lugar de eliminarla.

## Notas
- Relaciones: enviar ids en DTOs.
- Si hay errores de migraciones, crear y aplicar migraciones con EF Core.

---

Generado automáticamente por el asistente.
