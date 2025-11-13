using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaDto>> GetAllAsync();
        Task<AreaDto?> GetByIdAsync(int id);
        Task<AreaDto> CreateAsync(AreaDto dto);
        Task<AreaDto?> UpdateAsync(int id, AreaDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
