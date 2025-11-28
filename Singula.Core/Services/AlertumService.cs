using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Infrastructure.Data;

namespace Singula.Core.Services
{
    public class AlertumService : IAlertumService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        // CONSTRUCTOR REFACTORIZADO: Solo inyectamos IServiceScopeFactory
        public AlertumService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // --- MÉTODO DE SINCRONIZACIÓN (Feature: backend-notificaciones) ---
        public async Task SincronizarAlertasAutomaticas()
        {
            // CATCH GLOBAL para evitar el crash del proceso
            try
            {
                // CREAR SCOPE AISLADO: Esto crea una instancia fresca y segura de DbContext
                using (var scope = _scopeFactory.CreateScope())
                {
                    // Obtener instancias de los servicios dentro del scope
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // 1. TRAER SOLICITUDES ACTIVAS (con AsNoTracking para optimizar lectura)
                    var solicitudesActivas = await context.Solicituds
                        .AsNoTracking()
                        .Include(s => s.IdSlaNavigation)
                        .Include(s => s.IdRolRegistroNavigation)
                        .Include(s => s.IdEstadoSolicitudNavigation)
                        .Where(s => s.IdEstadoSolicitudNavigation.Codigo != "EST_FIN" &&
                                    s.IdEstadoSolicitudNavigation.Codigo != "EST_CANC")
                        .ToListAsync();

                    // 2. TRAER ALERTAS EXISTENTES
                    var alertasExistentesData = await context.Alerta
                        .AsNoTracking()
                        .Select(a => new
                        {
                            a.IdAlerta,
                            a.IdSolicitud,
                            a.IdTipoAlerta,
                            a.IdEstadoAlerta
                        })
                        .ToListAsync();

                    // Crear diccionario para búsqueda eficiente
                    var alertasDict = alertasExistentesData.ToDictionary(a => a.IdSolicitud);

                    var hoy = DateTime.UtcNow;
                    
                    // Listas para acumular operaciones
                    var alertasParaCrear = new List<Alertum>();
                    var alertasParaActualizar = new List<(int IdAlerta, int NuevoTipoAlerta, int NuevoEstadoAlerta, string NuevoNivel, string NuevoMensaje)>();

                    foreach (var sol in solicitudesActivas)
                    {
                        var fechaInicio = sol.FechaSolicitud ?? hoy;
                        var diasLimite = sol.IdSlaNavigation.DiasUmbral ?? 0;
                        var diasTranscurridos = (int)(hoy - fechaInicio).TotalDays;
                        var diasRestantes = diasLimite - diasTranscurridos;

                        int? nuevoTipoAlerta = null;
                        string nuevoMensaje = "";
                        string nuevoNivel = "";

                        // CALCULAR TIPO
                        if (diasRestantes <= 0) // ROJO
                        {
                            nuevoTipoAlerta = 2;
                            nuevoNivel = "Crítico";
                            int diasRetraso = Math.Abs(diasRestantes);
                            nuevoMensaje = $"Incumplimiento de {sol.IdSlaNavigation.CodigoSla} para {sol.IdRolRegistroNavigation.NombreRol}: {diasRetraso} días acumulados de retraso.";
                        }
                        else if (diasRestantes <= 2) // NARANJA - POR VENCER (2 DÍAS O MENOS)
                        {
                            nuevoTipoAlerta = 1;
                            nuevoNivel = "Alto";
                            nuevoMensaje = $"La solicitud de {sol.IdSlaNavigation.CodigoSla} para {sol.IdRolRegistroNavigation.NombreRol} está por vencer en: {diasRestantes} días.";
                        }

                        if (nuevoTipoAlerta == null) continue;

                        // Buscar alerta existente en el diccionario
                        if (!alertasDict.TryGetValue(sol.IdSolicitud, out var alertaData))
                        {
                            // CREAR NUEVA (Siempre nace como NO LEÍDA = 1)
                            var nuevaAlerta = new Alertum
                            {
                                IdSolicitud = sol.IdSolicitud,
                                IdTipoAlerta = nuevoTipoAlerta.Value,
                                IdEstadoAlerta = 1,
                                Nivel = nuevoNivel,
                                Mensaje = nuevoMensaje,
                                EnviadoEmail = false,
                                FechaCreacion = hoy
                            };
                            alertasParaCrear.Add(nuevaAlerta);
                        }
                        else
                        {
                            // LÓGICA DE ACTUALIZACIÓN
                            int nuevoEstadoAlerta = alertaData.IdEstadoAlerta;
                            
                            // CASO A: Es RIESGO (1). Revive siempre para avisar.
                            if (nuevoTipoAlerta == 1)
                            {
                                nuevoEstadoAlerta = 1; 
                            }

                            // CASO B: Es INCUMPLIMIENTO (2). Solo revive si escala de Naranja a Rojo.
                            if (nuevoTipoAlerta == 2 && alertaData.IdTipoAlerta == 1)
                            {
                                nuevoEstadoAlerta = 1;
                            }

                            // Acumular datos para actualización
                            alertasParaActualizar.Add((
                                alertaData.IdAlerta,
                                nuevoTipoAlerta.Value,
                                nuevoEstadoAlerta,
                                nuevoNivel,
                                nuevoMensaje
                            ));
                        }
                    }

                    // 3. EJECUTAR OPERACIONES DE ESCRITURA FUERA DEL BUCLE
                    
                    // Agregar nuevas alertas
                    if (alertasParaCrear.Any())
                    {
                        await context.Alerta.AddRangeAsync(alertasParaCrear);
                    }

                    // Actualizar alertas existentes
                    if (alertasParaActualizar.Any())
                    {
                        // Cargar solo las alertas que necesitan actualización (con tracking)
                        var idsParaActualizar = alertasParaActualizar.Select(a => a.IdAlerta).ToList();
                        var alertasRastreadas = await context.Alerta
                            .Where(a => idsParaActualizar.Contains(a.IdAlerta))
                            .ToListAsync();

                        // Crear diccionario para acceso rápido
                        var alertasRastreadasDict = alertasRastreadas.ToDictionary(a => a.IdAlerta);

                        foreach (var actualizacion in alertasParaActualizar)
                        {
                            if (alertasRastreadasDict.TryGetValue(actualizacion.IdAlerta, out var alertaEntity))
                            {
                                alertaEntity.IdTipoAlerta = actualizacion.NuevoTipoAlerta;
                                alertaEntity.IdEstadoAlerta = actualizacion.NuevoEstadoAlerta;
                                alertaEntity.Nivel = actualizacion.NuevoNivel;
                                alertaEntity.Mensaje = actualizacion.NuevoMensaje;
                                alertaEntity.ActualizadoEn = hoy;
                                
                                // Marcar explícitamente como modificado
                                context.Entry(alertaEntity).State = EntityState.Modified;
                            }
                        }
                    }

                    // 4. GUARDAR TODOS LOS CAMBIOS EN UNA SOLA TRANSACCIÓN
                    if (alertasParaCrear.Any() || alertasParaActualizar.Any())
                    {
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Registrar error - La excepción NO debe salir de este método para evitar crash
                Console.WriteLine($"[CRITICAL] FATAL CRASH PREVENTED in AlertumService.SincronizarAlertasAutomaticas: {ex.ToString()}");
                // Opcional: También puedes registrar en un archivo de log si tienes ILogger
            }
        }

        // --- MÉTODOS CRUD (Genéricos) ---
        // Estos métodos ahora también crean sus propios scopes para seguridad

        public async Task<AlertumDto> CreateAsync(AlertumDto dto)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRepository<Alertum>>();
                
                var entity = new Alertum
                {
                    IdSolicitud = dto.IdSolicitud,
                    IdTipoAlerta = dto.IdTipoAlerta,
                    IdEstadoAlerta = dto.IdEstadoAlerta,
                    Nivel = dto.Nivel,
                    Mensaje = dto.Mensaje,
                    EnviadoEmail = dto.EnviadoEmail
                };
                var created = await repo.CreateAsync(entity);
                return MapToDto(created);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRepository<Alertum>>();
                return await repo.DeleteAsync(id);
            }
        }

        public async Task<IEnumerable<AlertumDto>> GetAllAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRepository<Alertum>>();
                var list = await repo.GetAllAsync();
                return list.Select(MapToDto);
            }
        }

        public async Task<AlertumDto?> GetByIdAsync(int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRepository<Alertum>>();
                var e = await repo.GetByIdAsync(id);
                return e == null ? null : MapToDto(e);
            }
        }

        public async Task<AlertumDto?> UpdateAsync(int id, AlertumDto dto)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRepository<Alertum>>();
                var e = await repo.GetByIdAsync(id);
                if (e == null) return null;
                
                e.IdSolicitud = dto.IdSolicitud;
                e.IdTipoAlerta = dto.IdTipoAlerta;
                e.IdEstadoAlerta = dto.IdEstadoAlerta;
                e.Nivel = dto.Nivel;
                e.Mensaje = dto.Mensaje;
                e.EnviadoEmail = dto.EnviadoEmail;
                e.ActualizadoEn = dto.ActualizadoEn ?? DateTime.UtcNow;
                
                await repo.UpdateAsync(e);
                return dto;
            }
        }

        // --- MÉTODOS DELEGADOS (Master: Consultas por usuario) ---
        // Estos métodos utilizan el IAlertumRepository específico para consultas optimizadas

        public async Task<IEnumerable<AlertumDto>> GetByUserAsync(int userId, bool onlyUnread = false, int page = 1, int pageSize = 20)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertumRepository>();
                return await alertRepo.GetByUserAsync(userId, onlyUnread, page, pageSize);
            }
        }

        public async Task<int> GetUnreadCountByUserAsync(int userId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertumRepository>();
                return await alertRepo.GetUnreadCountByUserAsync(userId);
            }
        }

        public async Task<bool> MarkAsReadAsync(int alertId, int userId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var alertRepo = scope.ServiceProvider.GetRequiredService<IAlertumRepository>();
                var updated = await alertRepo.MarkAsReadAsync(alertId, userId);
                return updated != null;
            }
        }

        // Helper para mapeo limpio
        private static AlertumDto MapToDto(Alertum e)
        {
            return new AlertumDto
            {
                IdAlerta = e.IdAlerta,
                IdSolicitud = e.IdSolicitud,
                IdTipoAlerta = e.IdTipoAlerta,
                IdEstadoAlerta = e.IdEstadoAlerta,
                Nivel = e.Nivel,
                Mensaje = e.Mensaje,
                EnviadoEmail = e.EnviadoEmail,
                FechaCreacion = e.FechaCreacion,
                FechaLectura = e.FechaLectura,
                ActualizadoEn = e.ActualizadoEn
            };
        }
    }
}