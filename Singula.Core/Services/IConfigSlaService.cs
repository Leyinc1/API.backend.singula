using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IConfigSlaService
    {
        Task<IEnumerable<ConfigSlaDto>> GetAllAsync();
        Task<ConfigSlaDto?> GetByIdAsync(int id);
        Task<ConfigSlaDto> CreateAsync(ConfigSlaDto dto);
        Task<ConfigSlaDto?> UpdateAsync(int id, ConfigSlaDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
