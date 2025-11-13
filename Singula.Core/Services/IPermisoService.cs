using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IPermisoService
    {
        Task<IEnumerable<PermisoDto>> GetAllAsync();
        Task<PermisoDto?> GetByIdAsync(int id);
        Task<PermisoDto> CreateAsync(PermisoDto dto);
        Task<PermisoDto?> UpdateAsync(int id, PermisoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
