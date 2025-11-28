using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IPrioridadCatalogoService
    {
        Task<IEnumerable<PrioridadCatalogoDto>> GetAllAsync();
        Task<PrioridadCatalogoDto?> GetByIdAsync(int id);
        Task<PrioridadCatalogoDto> CreateAsync(PrioridadCatalogoDto dto);
        Task<PrioridadCatalogoDto?> UpdateAsync(int id, PrioridadCatalogoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
