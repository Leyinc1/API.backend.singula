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
            string? area = null,
            string? tipoSolicitud = null,
            string? prioridad = null,
            bool? cumpleSla = null)
        {
            var query = _context.Solicituds
                .Include(s => s.IdPersonalNavigation)
                .Include(s => s.IdAreaNavigation)
                .Include(s => s.IdSlaNavigation)
                    .ThenInclude(sla => sla!.IdTipoSolicitudNavigation)
                .AsQueryable();

            // Aplicar filtros
            if (startDate.HasValue)
                query = query.Where(s => s.FechaSolicitud >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(s => s.FechaSolicitud <= endDate.Value);

            if (!string.IsNullOrEmpty(area))
                query = query.Where(s => s.IdAreaNavigation.NombreArea == area);

            if (!string.IsNullOrEmpty(tipoSolicitud))
            {
                // Filtrar por descripción del tipo de solicitud (ej: "Nuevo Personal", "Reemplazo")
                // NOTA: El frontend puede enviar la descripción directamente
                query = query.Where(s => s.IdSlaNavigation.IdTipoSolicitudNavigation.Descripcion == tipoSolicitud);
            }

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

                // Determinar cumplimiento genérico
                var cumpleSla = diasTranscurridos <= diasUmbral;
                
                // Mantener lógica legacy para compatibilidad
                var cumpleSla1 = tipoSol == "Nuevo Personal" && cumpleSla;
                var cumpleSla2 = tipoSol == "Reemplazo" && cumpleSla;

                return new DashboardSlaDto
                {
                    Id = s.IdSolicitud,
                    Area = s.IdAreaNavigation?.NombreArea ?? "Sin Especificar",
                    TipoSolicitud = tipoSol,
                    Prioridad = s.Prioridad ?? "Media",
                    FechaSolicitud = s.FechaSolicitud,
                    FechaIngreso = s.FechaIngreso,
                    DiasTranscurridos = diasTranscurridos,
                    CumpleSla = cumpleSla,
                    CumpleSla1 = cumpleSla1,
                    CumpleSla2 = cumpleSla2,
                    NombrePersonal = $"{s.IdPersonalNavigation?.Nombres ?? ""} {s.IdPersonalNavigation?.Apellidos ?? ""}".Trim(),
                    DiasUmbralSla = diasUmbral
                };
            }).ToList();

            // Filtro adicional por cumplimiento
            if (cumpleSla.HasValue)
            {
                result = result.Where(r => r.CumpleSla == cumpleSla.Value).ToList();
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

            // Obtener todos los tipos de solicitud únicos
            var tiposSolicitud = solicitudes
                .Where(s => s.IdSlaNavigation?.IdTipoSolicitudNavigation?.Descripcion != null)
                .Select(s => s.IdSlaNavigation!.IdTipoSolicitudNavigation!.Descripcion!)
                .Distinct()
                .ToList();

            // Calcular estadísticas dinámicamente para cada tipo
            var estadisticasPorTipo = new Dictionary<string, SlaStats>();

            foreach (var tipo in tiposSolicitud)
            {
                var solicitudesTipo = solicitudes
                    .Where(s => s.IdSlaNavigation?.IdTipoSolicitudNavigation?.Descripcion == tipo)
                    .ToList();

                if (!solicitudesTipo.Any()) continue;

                var cumpleSlaCount = solicitudesTipo.Count(s =>
                {
                    var dias = s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                        ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                        : s.NumDiasSla ?? 0;
                    return dias <= (s.IdSlaNavigation?.DiasUmbral ?? 0);
                });

                var promedioDias = solicitudesTipo.Average(s =>
                    s.FechaIngreso.HasValue && s.FechaSolicitud.HasValue
                        ? (s.FechaIngreso.Value - s.FechaSolicitud.Value).Days
                        : s.NumDiasSla ?? 0);

                var porcentajeCumplimiento = (double)cumpleSlaCount / solicitudesTipo.Count * 100;

                // Obtener días umbral promedio para este tipo
                var diasUmbral = solicitudesTipo
                    .Select(s => s.IdSlaNavigation?.DiasUmbral ?? 0)
                    .FirstOrDefault();

                estadisticasPorTipo[tipo] = new SlaStats
                {
                    TotalSolicitudes = solicitudesTipo.Count,
                    CumpleSla = cumpleSlaCount,
                    PorcentajeCumplimiento = porcentajeCumplimiento,
                    PromedioDias = promedioDias,
                    DiasUmbral = diasUmbral
                };
            }

            // Mantener campos legacy para compatibilidad
            var cumplimientoSla1 = estadisticasPorTipo.ContainsKey("Nuevo Personal") 
                ? estadisticasPorTipo["Nuevo Personal"].PorcentajeCumplimiento 
                : 0;
            var cumplimientoSla2 = estadisticasPorTipo.ContainsKey("Reemplazo") 
                ? estadisticasPorTipo["Reemplazo"].PorcentajeCumplimiento 
                : 0;
            var promedioDiasSla1 = estadisticasPorTipo.ContainsKey("Nuevo Personal") 
                ? estadisticasPorTipo["Nuevo Personal"].PromedioDias 
                : 0;
            var promedioDiasSla2 = estadisticasPorTipo.ContainsKey("Reemplazo") 
                ? estadisticasPorTipo["Reemplazo"].PromedioDias 
                : 0;

            return new DashboardStatsDto
            {
                TotalSolicitudes = totalSolicitudes,
                EstadisticasPorTipo = estadisticasPorTipo,
                CumplimientoSla1 = cumplimientoSla1,
                CumplimientoSla2 = cumplimientoSla2,
                PromedioDiasSla1 = promedioDiasSla1,
                PromedioDiasSla2 = promedioDiasSla2
            };
        }

        public async Task<DashboardFiltersDto> GetAvailableFiltersAsync()
        {
            // Obtener áreas únicas
            var areas = await _context.Areas
                .Where(a => !string.IsNullOrEmpty(a.NombreArea))
                .Select(a => a.NombreArea!)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            // Obtener tipos de solicitud únicos
            var tiposSolicitud = await _context.TipoSolicitudCatalogos
                .Select(t => t.Descripcion!)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            // Obtener prioridades únicas desde el catálogo (usando código)
            var prioridades = await _context.PrioridadCatalogos
                .Where(p => !string.IsNullOrEmpty(p.Codigo))
                .Select(p => p.Codigo!)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            // Obtener configuraciones SLA
            var configuracionesSla = await _context.ConfigSlas
                .Include(c => c.IdTipoSolicitudNavigation)
                .Select(c => new ConfigSlaInfo
                {
                    Id = c.IdSla,
                    CodigoSla = c.CodigoSla ?? "",
                    TipoSolicitud = c.IdTipoSolicitudNavigation != null ? c.IdTipoSolicitudNavigation.Descripcion ?? "" : "",
                    DiasUmbral = c.DiasUmbral ?? 0
                })
                .ToListAsync();

            return new DashboardFiltersDto
            {
                Areas = areas,
                TiposSolicitud = tiposSolicitud,
                Prioridades = prioridades,
                ConfiguracionesSla = configuracionesSla
            };
        }
    }
}
