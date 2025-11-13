using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface ISolicitudService
    {
        Task<IEnumerable<SolicitudDto>> GetAllAsync();
        Task<SolicitudDto?> GetByIdAsync(int id);
        Task<SolicitudDto> CreateAsync(SolicitudDto dto);
        Task<SolicitudDto?> UpdateAsync(int id, SolicitudDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
