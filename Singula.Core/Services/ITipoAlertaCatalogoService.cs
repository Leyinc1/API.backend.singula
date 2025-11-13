using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface ITipoAlertaCatalogoService
    {
        Task<IEnumerable<TipoAlertaCatalogoDto>> GetAllAsync();
        Task<TipoAlertaCatalogoDto?> GetByIdAsync(int id);
        Task<TipoAlertaCatalogoDto> CreateAsync(TipoAlertaCatalogoDto dto);
        Task<TipoAlertaCatalogoDto?> UpdateAsync(int id, TipoAlertaCatalogoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
