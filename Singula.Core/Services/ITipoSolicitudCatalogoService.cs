using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface ITipoSolicitudCatalogoService
    {
        Task<IEnumerable<TipoSolicitudCatalogoDto>> GetAllAsync();
        Task<TipoSolicitudCatalogoDto?> GetByIdAsync(int id);
        Task<TipoSolicitudCatalogoDto> CreateAsync(TipoSolicitudCatalogoDto dto);
        Task<TipoSolicitudCatalogoDto?> UpdateAsync(int id, TipoSolicitudCatalogoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
