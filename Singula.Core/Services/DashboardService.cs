using Microsoft.EntityFrameworkCore;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DashboardSlaDto>> GetDashboardDataAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? bloqueTech = null,
            string? tipoSolicitud = null,
            string? prioridad = null,
            bool? cumpleSla = null)
        {
            var query = _context.Solicituds
                .Include(s => s.IdPersonalNavigation)
                .Include(s => s.IdRolRegistroNavigation)
                .Include(s => s.IdSlaNavigation)
                    .ThenInclude(sla => sla!.IdTipoSolicitudNavigation)
                .AsQueryable();

            // Aplicar filtros
            if (startDate.HasValue)
                query = query.Where(s => s.FechaSolicitud >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.FechaSolicitud <= endDate.Value);

            if (!string.IsNullOrEmpty(bloqueTech))
                query = query.Where(s => s.IdRolRegistroNavigation.BloqueTech == bloqueTech);

            if (!string.IsNullOrEmpty(tipoSolicitud))
                query = query.Where(s => s.IdSlaNavigation.IdTipoSolicitudNavigation.Descripcion == tipoSolicitud);

            if (!string.IsNullOrEmpty(prioridad))
                query = query.Where(s => s.Prioridad == prioridad);

            var solicitudes = await query.ToListAsync();

            var result = solicitudes.Select(s =>
            {
                // Calcular días transcurridos
                var diasTranscurridos = s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                    ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                    : s.NumDiasSla ?? 0;

                var diasUmbral = s.IdSlaNavigation?.DiasUmbral ?? 0;
                var tipoSol = s.IdSlaNavigation?.IdTipoSolicitudNavigation?.Descripcion ?? "";

                // Determinar cumplimiento según el tipo
                var cumpleSla1 = tipoSol == "Nuevo Personal" && diasTranscurridos <= diasUmbral;
                var cumpleSla2 = tipoSol == "Reemplazo" && diasTranscurridos <= diasUmbral;

                return new DashboardSlaDto
                {
                    Id = s.IdSolicitud,
                    BloqueTech = s.IdRolRegistroNavigation?.BloqueTech ?? "Sin Especificar",
                    TipoSolicitud = tipoSol,
                    Prioridad = s.Prioridad ?? "Media",
                    FechaSolicitud = s.FechaSolicitud,
                    FechaIngreso = s.FechaIngreso,
                    DiasTranscurridos = diasTranscurridos,
                    CumpleSla1 = cumpleSla1,
                    CumpleSla2 = cumpleSla2,
                    NombrePersonal = $"{s.IdPersonalNavigation?.Nombres ?? ""} {s.IdPersonalNavigation?.Apellidos ?? ""}".Trim(),
                    DiasUmbralSla = diasUmbral
                };
            }).ToList();

            // Filtro adicional por cumplimiento
            if (cumpleSla.HasValue)
            {
                result = result.Where(r =>
                    (r.TipoSolicitud == "Nuevo Personal" && r.CumpleSla1 == cumpleSla.Value) ||
                    (r.TipoSolicitud == "Reemplazo" && r.CumpleSla2 == cumpleSla.Value)
                ).ToList();
            }

            return result;
        }

        public async Task<DashboardStatsDto> GetStatisticsAsync()
        {
            var solicitudes = await _context.Solicituds
                .Include(s => s.IdSlaNavigation)
                    .ThenInclude(sla => sla!.IdTipoSolicitudNavigation)
                .ToListAsync();

            var totalSolicitudes = solicitudes.Count;

            // SLA1 - Nuevo Personal
            var sla1Solicitudes = solicitudes
                .Where(s => s.IdSlaNavigation?.IdTipoSolicitudNavigation?.Descripcion == "Nuevo Personal")
                .ToList();

            var cumpleSla1Count = sla1Solicitudes.Count(s =>
            {
                var dias = s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                    ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                    : s.NumDiasSla ?? 0;
                return dias <= (s.IdSlaNavigation?.DiasUmbral ?? 0);
            });

            var promedioDiasSla1 = sla1Solicitudes.Any()
                ? sla1Solicitudes.Average(s =>
                    s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                        ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                        : s.NumDiasSla ?? 0)
                : 0;

            var cumplimientoSla1 = sla1Solicitudes.Any()
                ? (double)cumpleSla1Count / sla1Solicitudes.Count * 100
                : 0;

            // SLA2 - Reemplazo
            var sla2Solicitudes = solicitudes
                .Where(s => s.IdSlaNavigation?.IdTipoSolicitudNavigation?.Descripcion == "Reemplazo")
                .ToList();

            var cumpleSla2Count = sla2Solicitudes.Count(s =>
            {
                var dias = s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                    ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                    : s.NumDiasSla ?? 0;
                return dias <= (s.IdSlaNavigation?.DiasUmbral ?? 0);
            });

            var promedioDiasSla2 = sla2Solicitudes.Any()
                ? sla2Solicitudes.Average(s =>
                    s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                        ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                        : s.NumDiasSla ?? 0)
                : 0;

            var cumplimientoSla2 = sla2Solicitudes.Any()
                ? (double)cumpleSla2Count / sla2Solicitudes.Count * 100
                : 0;

            return new DashboardStatsDto
            {
                TotalSolicitudes = totalSolicitudes,
                CumplimientoSla1 = cumplimientoSla1,
                CumplimientoSla2 = cumplimientoSla2,
                PromedioDiasSla1 = promedioDiasSla1,
                PromedioDiasSla2 = promedioDiasSla2
            };
        }
    }
}
