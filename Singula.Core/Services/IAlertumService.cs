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

        // New operations
        Task<IEnumerable<AlertumDto>> GetByUserAsync(int userId, bool onlyUnread = false, int page = 1, int pageSize = 20);
        Task<int> GetUnreadCountByUserAsync(int userId);
        Task<bool> MarkAsReadAsync(int alertId, int userId);
    }
}
