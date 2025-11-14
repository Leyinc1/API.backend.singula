# Manual de Usuario

Este manual explica cómo usar la API REST del proyecto `API.backend.singula` desde el punto de vista de un consumidor (desarrollador, integrador o tester). Cubre todos los recursos (entidades) expuestos por los controladores del proyecto y muestra ejemplos de uso.

Resumen rápido
- Base URL: `http://{host}:{port}/api` (el puerto se muestra al ejecutar el proyecto).
- Formato: JSON para entrada y salida.
- Autenticación: puede usar JWT si está habilitada (revisar `POST /api/auth/login` o controlador equivalente). Añadir encabezado `Authorization: Bearer {token}` para endpoints protegidos.
- Colecciones Postman: `postman/*.json` incluidas en el repositorio.

Requisitos previos
- .NET 10 SDK instalado.
- Cadena de conexión a la base de datos configurada en `API.backend.singula/appsettings.json` o mediante variables de entorno.
- Herramienta para probar APIs: Postman, curl o HTTPie.

Ejecución local
1. Abrir terminal en la carpeta raíz del proyecto (la que contiene `API.backend.singula.csproj`).
2. Restaurar y compilar:
   - `dotnet restore`
   - `dotnet build`
3. Ejecutar:
   - `dotnet run --project API.backend.singula/API.backend.singula.csproj`
4. Acceder a `http://localhost:{port}`. Si Swagger/OpenAPI está activado, se puede usar `http://localhost:{port}/swagger`.

Autenticación
- Si la API expone un endpoint de auth (por ejemplo `POST /api/auth/login`), use las credenciales para obtener un JWT.
- Añada el header `Authorization: Bearer {token}` en las peticiones a endpoints protegidos.

Formatos de petición y respuesta
- Content-Type: `application/json`
- Todas las respuestas devuelven JSON con los objetos o errores según el código HTTP.

Recursos y endpoints principales

Nota: Las rutas descritas son aproximadas; consulte los controladores en `API.backend.singula/Controllers/` para las rutas exactas y parámetros.

1) Usuarios
- `GET /api/usuarios` — Obtener todos los usuarios.
- `GET /api/usuarios/{id}` — Obtener usuario por id.
- `POST /api/usuarios` — Crear usuario. Body: `CreateUsuarioDto`.
- `PUT /api/usuarios/{id}` — Actualizar usuario. Body: `UpdateUsuarioDto`.
- `DELETE /api/usuarios/{id}` — Eliminar usuario.
- `POST /api/usuarios/login` o `POST /api/auth/login` — Autenticación (si aplica).

2) Personal
- CRUD: `GET /api/personals`, `GET /api/personals/{id}`, `POST /api/personals`, `PUT /api/personals/{id}`, `DELETE /api/personals/{id}`.
- Relación: cada `Personal` está asociada a un `Usuario` (1:1 según modelo).

3) Áreas
- CRUD: `GET /api/areas`, `GET /api/areas/{id}`, `POST /api/areas`, `PUT /api/areas/{id}`, `DELETE /api/areas/{id}`.

4) RolesSistema
- CRUD: `GET /api/rolessistema`, `GET /api/rolessistema/{id}`, `POST /api/rolessistema`, `PUT /api/rolessistema/{id}`, `DELETE /api/rolessistema/{id}`.

5) RolRegistro
- CRUD para registros de roles y asignaciones.

6) ConfigSla
- CRUD para configuraciones SLA por tipo de solicitud.
- `GET /api/configsla` — listar, `POST` crear, etc.

7) Catálogos y estados
- `TipoSolicitudCatalogo`, `TipoAlertaCatalogo`, `EstadoSolicitudCatalogo`, `EstadoAlertaCatalogo`, `EstadoUsuarioCatalogo`:
  - Endpoints CRUD para manejar catálogos usados por solicitudes y alertas.

8) Permisos
- CRUD para permisos del sistema. Pueden relacionarse con roles.

9) Alerta (Alertum)
- CRUD y endpoints específicos para crear alertas, listar por estado, marcar resueltas, etc.

10) Solicitud
- Endpoints para crear solicitudes, actualizar estado, consultar por filtros (por tipo, por estado, por usuario) y listar historiales.

11) Reporte
- Endpoints para generar y listar reportes. Generación puede devolver un `ruta_archivo` o similar para descargar el archivo.

DTOs y modelos de entrada
- Los DTOs se encuentran en `Singula.Core/Services/Dto/`.
- Ejemplos: `CreateUsuarioDto`, `UpdateUsuarioDto`, `UsuarioDto`, `SolicitudDto`, `ReporteDto`, `ConfigSlaDto`, `PersonalDto`, `AreaDto`, `RolesSistemaDto`, etc.

Ejemplos prácticos (curl)
- Obtener usuarios:
  curl -X GET "http://localhost:5000/api/usuarios" -H "Accept: application/json"

- Crear usuario:
  curl -X POST "http://localhost:5000/api/usuarios" -H "Content-Type: application/json" -d '{"username":"jdoe","correo":"jdoe@example.com","password":"Secret"}'

- Crear solicitud:
  curl -X POST "http://localhost:5000/api/solicitud" -H "Content-Type: application/json" -d '{"idPersonal":1,"idTipoSolicitud":2,"descripcion":"Requiere atención"}'

- Generar reporte (ejemplo):
  curl -X POST "http://localhost:5000/api/reportes" -H "Content-Type: application/json" -d '{"tipoReporte":"resumen","filtrosJson":"{...}"}'

Errores y códigos HTTP
- `200`/`201` — éxito.
- `204` — operación exitosa sin contenido (por ejemplo DELETE).
- `400` — petición inválida (valide DTOs y campos requeridos).
- `401` — no autenticado.
- `403` — no autorizado.
- `404` — recurso no encontrado.
- `500` — error interno del servidor.

Buenas prácticas para consumidores
- Validar respuestas y códigos HTTP.
- Usar timeouts y reintentos para operaciones idempotentes.
- Para operaciones que puedan tardar (generación de reportes), usar endpoints que devuelvan estado y ruta de descarga.

Colecciones Postman y automatización
- Importe las colecciones dentro de la carpeta `postman/`.
- Configure variables de entorno: `base_url`, `token`.
- Use Runner o Newman para pruebas automatizadas.

Soporte y reporte de incidencias
- Proporcione: pasos para reproducir, request (headers + body), respuesta (status + body), logs si es posible.

Fin del Manual de Usuario.
