using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.DTOs.Prediccion;

namespace Singula.Core.Core.Interfaces.Services;

public interface IPrediccionService
{
    Task<ApiResponse<PrediccionDTO>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<PrediccionDTO>>> GetAllAsync(PaginationParams paginationParams);
    Task<ApiResponse<PrediccionResultadoDTO>> GenerarPrediccionAsync(CreatePrediccionDTO dto);
    Task<ApiResponse<List<PrediccionDTO>>> GetByIndicadorAsync(string indicador);
    Task<ApiResponse<PrediccionDTO>> GetUltimaPrediccionAsync(string indicador);
}
