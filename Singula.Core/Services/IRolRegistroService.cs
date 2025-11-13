using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IRolRegistroService
    {
        Task<IEnumerable<RolRegistroDto>> GetAllAsync();
        Task<RolRegistroDto?> GetByIdAsync(int id);
        Task<RolRegistroDto> CreateAsync(RolRegistroDto dto);
        Task<RolRegistroDto?> UpdateAsync(int id, RolRegistroDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
