using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Infrastructure.Data;

namespace Singula.Core.Services
{
    public class AlertumService : IAlertumService
    {
        private readonly IRepository<Alertum> _repo;       // Para CRUD genérico
        private readonly ApplicationDbContext _context;    // Para lógica compleja de SLAs (Feature)
        private readonly IAlertumRepository _alertRepo;    // Para consultas específicas por usuario (Master)

        // CONSTRUCTOR UNIFICADO: Inyectamos las 3 dependencias
        public AlertumService(IRepository<Alertum> repo, ApplicationDbContext context, IAlertumRepository alertRepo)
        {
            _repo = repo;
            _context = context;
            _alertRepo = alertRepo;
        }

        // --- MÉTODO DE SINCRONIZACIÓN (Feature: backend-notificaciones) ---
        public async Task SincronizarAlertasAutomaticas()
        {
            // 1. TRAER SOLICITUDES ACTIVAS
            var solicitudesActivas = await _context.Solicituds
                .Include(s => s.IdSlaNavigation)
                .Include(s => s.IdRolRegistroNavigation)
                .Include(s => s.IdEstadoSolicitudNavigation)
                .Where(s => s.IdEstadoSolicitudNavigation.Codigo != "EST_FIN" &&
                            s.IdEstadoSolicitudNavigation.Codigo != "EST_CANC")
                .ToListAsync();

            // 2. TRAER TODAS LAS ALERTAS
            var alertasExistentes = await _context.Alerta.ToListAsync();
            var hoy = DateTime.UtcNow;

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
                else if (diasRestantes <= 10) // NARANJA
                {
                    nuevoTipoAlerta = 1;
                    nuevoNivel = "Alto";
                    nuevoMensaje = $"La solicitud de {sol.IdSlaNavigation.CodigoSla} para {sol.IdRolRegistroNavigation.NombreRol} está por vencer en: {diasRestantes} días.";
                }

                if (nuevoTipoAlerta == null) continue;

                var alertaExistente = alertasExistentes.FirstOrDefault(a => a.IdSolicitud == sol.IdSolicitud);

                if (alertaExistente == null)
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
                        FechaCreacion = DateTime.UtcNow
                    };
                    _context.Alerta.Add(nuevaAlerta);
                }
                else
                {
                    // LÓGICA DE ACTUALIZACIÓN
                    
                    // CASO A: Es RIESGO (1). Revive siempre para avisar.
                    if (nuevoTipoAlerta == 1)
                    {
                        alertaExistente.IdEstadoAlerta = 1; 
                    }

                    // CASO B: Es INCUMPLIMIENTO (2). Solo revive si escala de Naranja a Rojo.
                    if (nuevoTipoAlerta == 2 && alertaExistente.IdTipoAlerta == 1)
                    {
                        alertaExistente.IdEstadoAlerta = 1;
                    }

                    // Actualizar datos informativos
                    alertaExistente.IdTipoAlerta = nuevoTipoAlerta.Value;
                    alertaExistente.Nivel = nuevoNivel;
                    alertaExistente.Mensaje = nuevoMensaje;
                    alertaExistente.ActualizadoEn = DateTime.UtcNow;

                    _context.Alerta.Update(alertaExistente);
                }
            }

            await _context.SaveChangesAsync();
        }

        // --- MÉTODOS CRUD (Genéricos) ---

        public async Task<AlertumDto> CreateAsync(AlertumDto dto)
        {
            var entity = new Alertum
            {
                IdSolicitud = dto.IdSolicitud,
                IdTipoAlerta = dto.IdTipoAlerta,
                IdEstadoAlerta = dto.IdEstadoAlerta,
                Nivel = dto.Nivel,
                Mensaje = dto.Mensaje,
                EnviadoEmail = dto.EnviadoEmail
            };
            var created = await _repo.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<AlertumDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<AlertumDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<AlertumDto?> UpdateAsync(int id, AlertumDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            
            e.IdSolicitud = dto.IdSolicitud;
            e.IdTipoAlerta = dto.IdTipoAlerta;
            e.IdEstadoAlerta = dto.IdEstadoAlerta;
            e.Nivel = dto.Nivel;
            e.Mensaje = dto.Mensaje;
            e.EnviadoEmail = dto.EnviadoEmail;
            e.ActualizadoEn = dto.ActualizadoEn ?? DateTime.UtcNow;
            
            await _repo.UpdateAsync(e);
            return dto;
        }

        // --- MÉTODOS DELEGADOS (Master: Consultas por usuario) ---
        // Estos métodos utilizan el IAlertumRepository específico para consultas optimizadas

        public async Task<IEnumerable<AlertumDto>> GetByUserAsync(int userId, bool onlyUnread = false, int page = 1, int pageSize = 20)
            => await _alertRepo.GetByUserAsync(userId, onlyUnread, page, pageSize);

        public async Task<int> GetUnreadCountByUserAsync(int userId)
            => await _alertRepo.GetUnreadCountByUserAsync(userId);

        public async Task<bool> MarkAsReadAsync(int alertId, int userId)
        {
            var updated = await _alertRepo.MarkAsReadAsync(alertId, userId);
            return updated != null;
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