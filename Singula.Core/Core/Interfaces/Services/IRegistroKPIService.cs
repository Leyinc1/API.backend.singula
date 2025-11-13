using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.DTOs.RegistroKPI;

namespace Singula.Core.Core.Interfaces.Services;

public interface IRegistroKPIService
{
    Task<ApiResponse<RegistroKPIDTO>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<RegistroKPIDTO>>> GetAllAsync(PaginationParams paginationParams);
    Task<ApiResponse<RegistroKPIDTO>> CreateAsync(CreateRegistroKPIDTO dto);
    Task<ApiResponse<RegistroKPIDTO>> UpdateAsync(int id, UpdateRegistroKPIDTO dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<List<RegistroKPIDTO>>> GetByIndicadorAsync(string indicador);
    Task<ApiResponse<List<RegistroKPIDTO>>> GetByAreaAsync(string area);
    Task<ApiResponse<List<KPIDashboardDTO>>> GetDashboardDataAsync();
    Task<ApiResponse<KPIDashboardDTO>> GetKPIHistoricoAsync(string indicador, DateTime? desde = null, DateTime? hasta = null);
}
