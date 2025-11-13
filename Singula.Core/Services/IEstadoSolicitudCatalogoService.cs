using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface IEstadoSolicitudCatalogoService
    {
        Task<IEnumerable<EstadoSolicitudCatalogoDto>> GetAllAsync();
        Task<EstadoSolicitudCatalogoDto?> GetByIdAsync(int id);
        Task<EstadoSolicitudCatalogoDto> CreateAsync(EstadoSolicitudCatalogoDto dto);
        Task<EstadoSolicitudCatalogoDto?> UpdateAsync(int id, EstadoSolicitudCatalogoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
