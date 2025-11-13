using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IEstadoUsuarioCatalogoService
    {
        Task<IEnumerable<EstadoUsuarioCatalogoDto>> GetAllAsync();
        Task<EstadoUsuarioCatalogoDto?> GetByIdAsync(int id);
        Task<EstadoUsuarioCatalogoDto> CreateAsync(EstadoUsuarioCatalogoDto dto);
        Task<EstadoUsuarioCatalogoDto?> UpdateAsync(int id, EstadoUsuarioCatalogoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
