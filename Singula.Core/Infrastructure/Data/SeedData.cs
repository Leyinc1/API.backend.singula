using System;
using System.Linq;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Data
{
    public static class SeedData
    {
        public static void EnsureSeedData(ApplicationDbContext context)
        {
            // Ensure database is created (no-op if using migrations)
            try
            {
                context.Database.EnsureCreated();
            }
            catch
            {
                // ignore
            }

            // Seed RolesSistema if empty
            if (!context.RolesSistemas.Any())
            {
                context.RolesSistemas.AddRange(
                    new RolesSistema { IdRolSistema = 1, Codigo = "ADMIN", Nombre = "Administrador", Descripcion = "Administrador del sistema", EsActivo = true },
                    new RolesSistema { IdRolSistema = 2, Codigo = "USER", Nombre = "Usuario", Descripcion = "Usuario estándar", EsActivo = true },
                    new RolesSistema { IdRolSistema = 3, Codigo = "GUEST", Nombre = "Invitado", Descripcion = "Acceso limitado", EsActivo = true }
                );
                context.SaveChanges();
            }

            // Seed EstadoUsuarioCatalogo if empty
            if (!context.EstadoUsuarioCatalogos.Any())
            {
                context.EstadoUsuarioCatalogos.AddRange(
                    new EstadoUsuarioCatalogo { IdEstadoUsuario = 1, Codigo = "ACT", Descripcion = "Activo" },
                    new EstadoUsuarioCatalogo { IdEstadoUsuario = 2, Codigo = "INA", Descripcion = "Inactivo" },
                    new EstadoUsuarioCatalogo { IdEstadoUsuario = 3, Codigo = "PEN", Descripcion = "Pendiente" }
                );
                context.SaveChanges();
            }

            // Seed Areas
            if (!context.Areas.Any())
            {
                context.Areas.AddRange(
                    new Area { NombreArea = "Tecnología", Descripcion = "Área de Tecnología e Innovación" },
                    new Area { NombreArea = "Recursos Humanos", Descripcion = "Área de Gestión de Personas" }
                );
                context.SaveChanges();
            }

            // Seed TipoSolicitudCatalogo
            if (!context.TipoSolicitudCatalogos.Any())
            {
                context.TipoSolicitudCatalogos.AddRange(
                    new TipoSolicitudCatalogo { Codigo = "NUEVO_PERSONAL", Descripcion = "Nuevo Personal" },
                    new TipoSolicitudCatalogo { Codigo = "REEMPLAZO", Descripcion = "Reemplazo" }
                );
                context.SaveChanges();
            }

            // Seed EstadoSolicitudCatalogo
            if (!context.EstadoSolicitudCatalogos.Any())
            {
                context.EstadoSolicitudCatalogos.AddRange(
                    new EstadoSolicitudCatalogo { Codigo = "PENDIENTE", Descripcion = "Pendiente" },
                    new EstadoSolicitudCatalogo { Codigo = "EN_PROCESO", Descripcion = "En Proceso" },
                    new EstadoSolicitudCatalogo { Codigo = "COMPLETADA", Descripcion = "Completada" },
                    new EstadoSolicitudCatalogo { Codigo = "RECHAZADA", Descripcion = "Rechazada" }
                );
                context.SaveChanges();
            }

            // Seed RolRegistro con bloques tech
            if (!context.RolRegistros.Any())
            {
                context.RolRegistros.AddRange(
                    new RolRegistro { NombreRol = "Desarrollador Backend", BloqueTech = "Backend", Descripcion = "Desarrollo de servicios backend", EsActivo = true },
                    new RolRegistro { NombreRol = "Desarrollador Frontend", BloqueTech = "Frontend", Descripcion = "Desarrollo de interfaces de usuario", EsActivo = true },
                    new RolRegistro { NombreRol = "Analista QA", BloqueTech = "QA", Descripcion = "Control de calidad y testing", EsActivo = true },
                    new RolRegistro { NombreRol = "Desarrollador Mobile", BloqueTech = "Mobile", Descripcion = "Desarrollo de aplicaciones móviles", EsActivo = true },
                    new RolRegistro { NombreRol = "Ingeniero DevOps", BloqueTech = "DevOps", Descripcion = "Automatización y despliegue", EsActivo = true },
                    new RolRegistro { NombreRol = "Científico de Datos", BloqueTech = "Data", Descripcion = "Análisis y ciencia de datos", EsActivo = true }
                );
                context.SaveChanges();
            }

            // Seed ConfigSla
            if (!context.ConfigSlas.Any())
            {
                var tipoNuevoPersonal = context.TipoSolicitudCatalogos.FirstOrDefault(t => t.Codigo == "NUEVO_PERSONAL");
                var tipoReemplazo = context.TipoSolicitudCatalogos.FirstOrDefault(t => t.Codigo == "REEMPLAZO");

                if (tipoNuevoPersonal != null)
                {
                    context.ConfigSlas.Add(new ConfigSla
                    {
                        CodigoSla = "SLA_NUEVO_35",
                        Descripcion = "SLA para contratación de nuevo personal - 35 días",
                        DiasUmbral = 35,
                        IdTipoSolicitud = tipoNuevoPersonal.IdTipoSolicitud,
                        EsActivo = true
                    });
                }

                if (tipoReemplazo != null)
                {
                    context.ConfigSlas.Add(new ConfigSla
                    {
                        CodigoSla = "SLA_REEMPLAZO_20",
                        Descripcion = "SLA para reemplazo de personal - 20 días",
                        DiasUmbral = 20,
                        IdTipoSolicitud = tipoReemplazo.IdTipoSolicitud,
                        EsActivo = true
                    });
                }
                context.SaveChanges();
            }

            // Seed Usuarios
            if (!context.Usuarios.Any())
            {
                var rolAdmin = context.RolesSistemas.FirstOrDefault(r => r.Codigo == "ADMIN");
                var rolUser = context.RolesSistemas.FirstOrDefault(r => r.Codigo == "USER");
                var estadoActivo = context.EstadoUsuarioCatalogos.FirstOrDefault(e => e.Codigo == "ACT");

                if (rolAdmin != null && estadoActivo != null)
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Username = "admin",
                        Correo = "admin@singula.com",
                        PasswordHash = "hash_password_admin",
                        IdRolSistema = rolAdmin.IdRolSistema,
                        IdEstadoUsuario = estadoActivo.IdEstadoUsuario
                    });
                }

                if (rolUser != null && estadoActivo != null)
                {
                    context.Usuarios.Add(new Usuario
                    {
                        Username = "rrhh",
                        Correo = "rrhh@singula.com",
                        PasswordHash = "hash_password_rrhh",
                        IdRolSistema = rolUser.IdRolSistema,
                        IdEstadoUsuario = estadoActivo.IdEstadoUsuario
                    });
                }
                context.SaveChanges();
            }

            // Seed Personal
            if (!context.Personals.Any())
            {
                var usuarioAdmin = context.Usuarios.FirstOrDefault(u => u.Username == "admin");
                var usuarioRrhh = context.Usuarios.FirstOrDefault(u => u.Username == "rrhh");

                if (usuarioAdmin != null)
                {
                    context.Personals.Add(new Personal
                    {
                        IdUsuario = usuarioAdmin.IdUsuario,
                        Nombres = "Juan Carlos",
                        Apellidos = "Pérez García",
                        Documento = "12345678",
                        Estado = "Activo"
                    });
                }

                if (usuarioRrhh != null)
                {
                    context.Personals.Add(new Personal
                    {
                        IdUsuario = usuarioRrhh.IdUsuario,
                        Nombres = "María Elena",
                        Apellidos = "Rodríguez López",
                        Documento = "87654321",
                        Estado = "Activo"
                    });
                }
                context.SaveChanges();
            }

            // Seed Solicitudes de prueba para el Dashboard
            if (!context.Solicituds.Any())
            {
                var area = context.Areas.FirstOrDefault();
                var estadoCompletada = context.EstadoSolicitudCatalogos.FirstOrDefault(e => e.Codigo == "COMPLETADA");
                var usuarioAdmin = context.Usuarios.FirstOrDefault(u => u.Username == "admin");
                var personal = context.Personals.FirstOrDefault();
                var roles = context.RolRegistros.ToList();
                var slaNuevo = context.ConfigSlas.FirstOrDefault(s => s.CodigoSla == "SLA_NUEVO_35");
                var slaReemplazo = context.ConfigSlas.FirstOrDefault(s => s.CodigoSla == "SLA_REEMPLAZO_20");

                if (area != null && estadoCompletada != null && usuarioAdmin != null && personal != null && roles.Any())
                {
                    var random = new Random();
                    var prioridades = new[] { "Crítica", "Alta", "Media", "Baja" };
                    var solicitudes = new System.Collections.Generic.List<Solicitud>();

                    for (int i = 0; i < 60; i++)
                    {
                        // Fecha aleatoria en los últimos 120 días
                        var diasAtras = random.Next(5, 120);
                        var fechaSolicitud = DateTime.UtcNow.AddDays(-diasAtras);

                        // Días transcurridos (5 a 50 días)
                        var diasTranscurridos = random.Next(5, 50);
                        var fechaIngreso = fechaSolicitud.AddDays(diasTranscurridos);

                        // Seleccionar rol aleatorio
                        var rol = roles[i % roles.Count];

                        // Alternar entre tipos de SLA
                        var sla = i % 2 == 0 ? slaNuevo : slaReemplazo;

                        if (sla != null)
                        {
                            solicitudes.Add(new Solicitud
                            {
                                IdPersonal = personal.IdPersonal,
                                IdRolRegistro = rol.IdRolRegistro,
                                IdSla = sla.IdSla,
                                IdArea = area.IdArea,
                                IdEstadoSolicitud = estadoCompletada.IdEstadoSolicitud,
                                FechaSolicitud = fechaSolicitud,
                                FechaIngreso = fechaIngreso,
                                NumDiasSla = diasTranscurridos,
                                Prioridad = prioridades[random.Next(prioridades.Length)],
                                ResumenSla = "Solicitud de prueba generada automáticamente",
                                OrigenDato = "SEED_DATA",
                                CreadoPor = usuarioAdmin.IdUsuario
                            });
                        }
                    }

                    context.Solicituds.AddRange(solicitudes);
                    context.SaveChanges();
                }
            }

            // Seed PrioridadCatalogo if empty
            if (!context.PrioridadCatalogos.Any())
            {
                context.PrioridadCatalogos.AddRange(
                    new PrioridadCatalogo
                    {
                        Codigo = "CRITICA",
                        Descripcion = "Prioridad Crítica - Requiere atención inmediata",
                        Nivel = 4,
                        SlaMultiplier = 0.5m, // Reduce el SLA a la mitad
                        Icon = "emergency",
                        Color = "#d32f2f",
                        Activo = true
                    },
                    new PrioridadCatalogo
                    {
                        Codigo = "ALTA",
                        Descripcion = "Prioridad Alta - Requiere atención prioritaria",
                        Nivel = 3,
                        SlaMultiplier = 0.75m, // Reduce el SLA un 25%
                        Icon = "priority_high",
                        Color = "#f57c00",
                        Activo = true
                    },
                    new PrioridadCatalogo
                    {
                        Codigo = "MEDIA",
                        Descripcion = "Prioridad Media - Atención en tiempo normal",
                        Nivel = 2,
                        SlaMultiplier = 1.0m, // SLA normal
                        Icon = "remove",
                        Color = "#1976d2",
                        Activo = true
                    },
                    new PrioridadCatalogo
                    {
                        Codigo = "BAJA",
                        Descripcion = "Prioridad Baja - Puede esperar",
                        Nivel = 1,
                        SlaMultiplier = 1.5m, // Aumenta el SLA un 50%
                        Icon = "arrow_downward",
                        Color = "#388e3c",
                        Activo = true
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
