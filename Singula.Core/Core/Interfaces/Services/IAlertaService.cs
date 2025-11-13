using Singula.Core.Core.DTOs.Alerta;
using Singula.Core.Core.DTOs.Common;

namespace Singula.Core.Core.Interfaces.Services;

public interface IAlertaService
{
    Task<ApiResponse<AlertaDTO>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<AlertaDTO>>> GetAllAsync(PaginationParams paginationParams);
    Task<ApiResponse<AlertaDTO>> CreateAsync(CreateAlertaDTO dto);
    Task<ApiResponse<AlertaDTO>> UpdateAsync(int id, UpdateAlertaDTO dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<List<AlertaDTO>>> GetNoLeidasAsync();
    Task<ApiResponse<AlertaResumenDTO>> GetResumenAsync();
    Task<ApiResponse<bool>> MarcarComoLeidaAsync(int id);
    Task<ApiResponse<bool>> EnviarAlertaPorEmailAsync(int id);
    Task GenerarAlertasAutomaticasAsync();
}
