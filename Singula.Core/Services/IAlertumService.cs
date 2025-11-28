using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IAlertumService
    {
        Task<IEnumerable<AlertumDto>> GetAllAsync();
        Task<AlertumDto?> GetByIdAsync(int id);
        Task<AlertumDto> CreateAsync(AlertumDto dto);
        Task<AlertumDto?> UpdateAsync(int id, AlertumDto dto);
        Task<bool> DeleteAsync(int id);
        Task SincronizarAlertasAutomaticas();
    }
}
