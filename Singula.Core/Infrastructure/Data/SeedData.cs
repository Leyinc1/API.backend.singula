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
            }

            // Seed EstadoUsuarioCatalogo if empty
            if (!context.EstadoUsuarioCatalogos.Any())
            {
                context.EstadoUsuarioCatalogos.AddRange(
                    new EstadoUsuarioCatalogo { IdEstadoUsuario = 1, Codigo = "ACT", Descripcion = "Activo" },
                    new EstadoUsuarioCatalogo { IdEstadoUsuario = 2, Codigo = "INA", Descripcion = "Inactivo" },
                    new EstadoUsuarioCatalogo { IdEstadoUsuario = 3, Codigo = "PEN", Descripcion = "Pendiente" }
                );
            }

            // Save changes if any
            if (context.ChangeTracker.HasChanges())
            {
                context.SaveChanges();
            }
        }
    }
}
