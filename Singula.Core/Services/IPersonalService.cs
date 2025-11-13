using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IPersonalService
    {
        Task<IEnumerable<PersonalDto>> GetAllAsync();
        Task<PersonalDto?> GetByIdAsync(int id);
        Task<PersonalDto> CreateAsync(PersonalDto dto);
        Task<PersonalDto?> UpdateAsync(int id, PersonalDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
