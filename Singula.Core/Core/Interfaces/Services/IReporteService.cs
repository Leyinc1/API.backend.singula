using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.DTOs.Reporte;

namespace Singula.Core.Core.Interfaces.Services;

public interface IReporteService
{
    Task<ApiResponse<ReporteDTO>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<ReporteDTO>>> GetAllAsync(PaginationParams paginationParams);
    Task<ApiResponse<ReporteDTO>> CreateAsync(CreateReporteDTO dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<ReporteGeneradoDTO>> GenerarReportePDFAsync(CreateReporteDTO dto);
    Task<ApiResponse<byte[]>> DescargarReporteAsync(int id);
}
