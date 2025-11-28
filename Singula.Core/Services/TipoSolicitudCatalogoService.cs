using Singula.Core.Repositories;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using System;

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
            var entity = new TipoSolicitudCatalogo 
            { 
                Codigo = dto.Codigo, 
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };
            var created = await _repo.CreateAsync(entity);
            return new TipoSolicitudCatalogoDto 
            { 
                IdTipoSolicitud = created.IdTipoSolicitud, 
                Codigo = created.Codigo, 
                Descripcion = created.Descripcion,
                Activo = created.Activo
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _repo.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                // Verificar si es un error de foreign key constraint
                if (ex.InnerException is PostgresException pgEx)
                {
                    if (pgEx.SqlState == "23503") // Foreign key violation
                    {
                        throw new InvalidOperationException(
                            "No se puede eliminar este tipo de solicitud porque existen solicitudes asociadas a él. " +
                            "Considere desactivarlo en lugar de eliminarlo.",
                            ex
                        );
                    }
                }
                throw; // Re-lanzar otros errores
            }
        }

        public async Task<IEnumerable<TipoSolicitudCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(t => new TipoSolicitudCatalogoDto 
            { 
                IdTipoSolicitud = t.IdTipoSolicitud, 
                Codigo = t.Codigo, 
                Descripcion = t.Descripcion,
                Activo = t.Activo
            });
        }

        public async Task<TipoSolicitudCatalogoDto?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            return new TipoSolicitudCatalogoDto 
            { 
                IdTipoSolicitud = e.IdTipoSolicitud, 
                Codigo = e.Codigo, 
                Descripcion = e.Descripcion,
                Activo = e.Activo
            };
        }

        public async Task<TipoSolicitudCatalogoDto?> UpdateAsync(int id, TipoSolicitudCatalogoDto dto)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;
            e.Codigo = dto.Codigo;
            e.Descripcion = dto.Descripcion;
            e.Activo = dto.Activo;
            await _repo.UpdateAsync(e);
            return new TipoSolicitudCatalogoDto
            {
                IdTipoSolicitud = e.IdTipoSolicitud,
                Codigo = e.Codigo,
                Descripcion = e.Descripcion,
                Activo = e.Activo
            };
        }
    }
}
