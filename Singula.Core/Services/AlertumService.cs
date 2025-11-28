using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Infrastructure.Data; // Asegúrate que este namespace sea correcto para tu DbContext

namespace Singula.Core.Services
{
    public class AlertumService : IAlertumService
    {
        private readonly IRepository<Alertum> _repo;
        private readonly ApplicationDbContext _context; // 1. Variable declarada

        // 2. CONSTRUCTOR CORREGIDO (Aquí estaba el error antes)
        public AlertumService(IRepository<Alertum> repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context; // <--- 3. ¡ASIGNACIÓN IMPORTANTE! Sin esto, _context es null.
        }

        // --- MÉTODO DE SINCRONIZACIÓN (Con Logs) ---
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

            // 2. TRAER TODAS LAS ALERTAS (Leídas y No Leídas)
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
                    // --- AQUÍ ESTÁ LA LÓGICA QUE PEDISTE ---

                    // CASO A: Es RIESGO (1). 
                    // Lógica: "Quiero seguir mostrándolas la próxima vez".
                    // Acción: Forzamos el estado a 1 (NO LEÍDO) siempre que se sincronice.
                    if (nuevoTipoAlerta == 1)
                    {
                        alertaExistente.IdEstadoAlerta = 1; // ¡Revive siempre!
                    }

                    // CASO B: Es INCUMPLIMIENTO (2).
                    // Lógica: "Si marqué como leído, ya no la quiero ver".
                    // Acción: NO tocamos el IdEstadoAlerta. Si estaba en 2, se queda en 2 (oculto).
                    // Solo si cambia de Naranja (1) a Rojo (2) se resetea por la lógica de abajo.
                    if (nuevoTipoAlerta == 2 && alertaExistente.IdTipoAlerta == 1)
                    {
                        // Si escaló de Riesgo a Incumplimiento, la revivimos para que se entere.
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

        // --- MÉTODOS CRUD (Sin cambios) ---

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
            return new AlertumDto
            {
                IdAlerta = created.IdAlerta,
                IdSolicitud = created.IdSolicitud,
                IdTipoAlerta = created.IdTipoAlerta,
                IdEstadoAlerta = created.IdEstadoAlerta,
                Nivel = created.Nivel,
                Mensaje = created.Mensaje,
                EnviadoEmail = created.EnviadoEmail,
                FechaCreacion = created.FechaCreacion,
                FechaLectura = created.FechaLectura,
                ActualizadoEn = created.ActualizadoEn
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<AlertumDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(a => new AlertumDto
            {
                IdAlerta = a.IdAlerta,
                IdSolicitud = a.IdSolicitud,
                IdTipoAlerta = a.IdTipoAlerta,
                IdEstadoAlerta = a.IdEstadoAlerta,
                Nivel = a.Nivel,
                Mensaje = a.Mensaje,
                EnviadoEmail = a.EnviadoEmail,
                FechaCreacion = a.FechaCreacion,
                FechaLectura = a.FechaLectura,
                ActualizadoEn = a.ActualizadoEn
            });
        }

        public async Task<AlertumDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
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
            e.ActualizadoEn = dto.ActualizadoEn;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}