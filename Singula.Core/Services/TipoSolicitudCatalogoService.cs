using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class TipoSolicitudCatalogoService : ITipoSolicitudCatalogoService
    {
        private readonly IRepository<TipoSolicitudCatalogo> _repo;

        public TipoSolicitudCatalogoService(IRepository<TipoSolicitudCatalogo> repo)
        {
            _repo = repo;
        }

        public async Task<TipoSolicitudCatalogoDto> CreateAsync(TipoSolicitudCatalogoDto dto)
        {
            var entity = new TipoSolicitudCatalogo { Codigo = dto.Codigo, Descripcion = dto.Descripcion };
            var created = await _repo.CreateAsync(entity);
            return new TipoSolicitudCatalogoDto { IdTipoSolicitud = created.IdTipoSolicitud, Codigo = created.Codigo, Descripcion = created.Descripcion };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<TipoSolicitudCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(t => new TipoSolicitudCatalogoDto { IdTipoSolicitud = t.IdTipoSolicitud, Codigo = t.Codigo, Descripcion = t.Descripcion });
        }

        public async Task<TipoSolicitudCatalogoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new TipoSolicitudCatalogoDto { IdTipoSolicitud = e.IdTipoSolicitud, Codigo = e.Codigo, Descripcion = e.Descripcion };
        }

        public async Task<TipoSolicitudCatalogoDto?> UpdateAsync(int id, TipoSolicitudCatalogoDto dto)
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
