using Singula.Core.Core.Entities;
using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Repositories
{
    public interface IAlertumRepository
    {
        Task<IEnumerable<Alertum>> GetAllAsync();
        Task<Alertum?> GetByIdAsync(int id);
        Task<Alertum> CreateAsync(Alertum entity);
        Task<Alertum?> UpdateAsync(Alertum entity);
        Task<bool> DeleteAsync(int id);

        // Specialized queries
        Task<IEnumerable<AlertumDto>> GetByUserAsync(int userId, bool onlyUnread, int page, int pageSize);
        Task<int> GetUnreadCountByUserAsync(int userId);
        Task<Alertum?> MarkAsReadAsync(int alertId, int userId);
    }
}