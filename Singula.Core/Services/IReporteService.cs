using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IReporteService
    {
        Task<IEnumerable<ReporteDto>> GetAllAsync();
        Task<ReporteDto?> GetByIdAsync(int id);
        Task<ReporteDto> CreateAsync(ReporteDto dto);
        Task<ReporteDto?> UpdateAsync(int id, ReporteDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
