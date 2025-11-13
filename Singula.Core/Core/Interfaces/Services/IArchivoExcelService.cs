using Singula.Core.Core.DTOs.ArchivoExcel;
using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.Entities;

namespace Singula.Core.Core.Interfaces.Services;

public interface IArchivoExcelService
{
    Task<ApiResponse<ArchivoExcelDTO>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<ArchivoExcelDTO>>> GetAllAsync(PaginationParams paginationParams);
    Task<ApiResponse<ArchivoExcelDTO>> CreateAsync(CreateArchivoExcelDTO dto);
    Task<ApiResponse<ArchivoExcelDTO>> UpdateAsync(int id, UpdateArchivoExcelDTO dto);
    Task<ApiResponse<bool>> DeleteAsync(int id);
    Task<ApiResponse<ArchivoExcelDTO>> ProcesarArchivoAsync(string rutaArchivo, int archivoId);
    Task<ApiResponse<List<ArchivoExcelDTO>>> GetRecentesAsync(int cantidad = 10);
    Task<ApiResponse<List<ArchivoExcelDTO>>> GetByEstadoAsync(EstadoProcesamiento estado);
}
