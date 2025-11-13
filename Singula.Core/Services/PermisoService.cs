using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly IRepository<Permiso> _repo;

        public PermisoService(IRepository<Permiso> repo)
        {
            _repo = repo;
        }

        public async Task<PermisoDto> CreateAsync(PermisoDto dto)
        {
            var entity = new Permiso { Codigo = dto.Codigo, Descripcion = dto.Descripcion, Nombre = dto.Nombre };
            var created = await _repo.CreateAsync(entity);
            return new PermisoDto { IdPermiso = created.IdPermiso, Codigo = created.Codigo, Descripcion = created.Descripcion, Nombre = created.Nombre };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<PermisoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(p => new PermisoDto { IdPermiso = p.IdPermiso, Codigo = p.Codigo, Descripcion = p.Descripcion, Nombre = p.Nombre });
        }

        public async Task<PermisoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new PermisoDto { IdPermiso = e.IdPermiso, Codigo = e.Codigo, Descripcion = e.Descripcion, Nombre = e.Nombre };
        }

        public async Task<PermisoDto?> UpdateAsync(int id, PermisoDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.Codigo = dto.Codigo;
            e.Descripcion = dto.Descripcion;
            e.Nombre = dto.Nombre;
            await _repo.UpdateAsync(e);
            return dto;
        }
    }
}
