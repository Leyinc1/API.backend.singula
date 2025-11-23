using Singula.Core.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IDashboardService
    {
        Task<IEnumerable<DashboardSlaDto>> GetDashboardDataAsync(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? bloqueTech = null,
            string? tipoSolicitud = null,
            string? prioridad = null,
            bool? cumpleSla = null
        );
        
        Task<DashboardStatsDto> GetStatisticsAsync();
    }
}
