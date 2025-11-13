using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IEstadoAlertaCatalogoService
    {
        Task<IEnumerable<EstadoAlertaCatalogoDto>> GetAllAsync();
        Task<EstadoAlertaCatalogoDto?> GetByIdAsync(int id);
        Task<EstadoAlertaCatalogoDto> CreateAsync(EstadoAlertaCatalogoDto dto);
        Task<EstadoAlertaCatalogoDto?> UpdateAsync(int id, EstadoAlertaCatalogoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
