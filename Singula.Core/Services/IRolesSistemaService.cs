using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IRolesSistemaService
    {
        Task<IEnumerable<RolesSistemaDto>> GetAllAsync();
        Task<RolesSistemaDto?> GetByIdAsync(int id);
        Task<RolesSistemaDto> CreateAsync(RolesSistemaDto dto);
        Task<RolesSistemaDto?> UpdateAsync(int id, RolesSistemaDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
