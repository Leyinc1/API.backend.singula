using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class EstadoSolicitudCatalogoService : IEstadoSolicitudCatalogoService
    {
        private readonly IRepository<EstadoSolicitudCatalogo> _repo;

        public EstadoSolicitudCatalogoService(IRepository<EstadoSolicitudCatalogo> repo)
        {
            _repo = repo;
        }

        public async Task<EstadoSolicitudCatalogoDto> CreateAsync(EstadoSolicitudCatalogoDto dto)
        {
            var entity = new EstadoSolicitudCatalogo { Codigo = dto.Codigo, Descripcion = dto.Descripcion };
            var created = await _repo.CreateAsync(entity);
            return new EstadoSolicitudCatalogoDto { IdEstadoSolicitud = created.IdEstadoSolicitud, Codigo = created.Codigo, Descripcion = created.Descripcion };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<EstadoSolicitudCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new EstadoSolicitudCatalogoDto { IdEstadoSolicitud = e.IdEstadoSolicitud, Codigo = e.Codigo, Descripcion = e.Descripcion });
        }

        public async Task<EstadoSolicitudCatalogoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new EstadoSolicitudCatalogoDto { IdEstadoSolicitud = e.IdEstadoSolicitud, Codigo = e.Codigo, Descripcion = e.Descripcion };
        }

        public async Task<EstadoSolicitudCatalogoDto?> UpdateAsync(int id, EstadoSolicitudCatalogoDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.Codigo = dto.Codigo;
            e.Descripcion = dto.Descripcion;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
