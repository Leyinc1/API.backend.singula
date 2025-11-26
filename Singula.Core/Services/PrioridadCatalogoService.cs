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
    public class PrioridadCatalogoService : IPrioridadCatalogoService
    {
        private readonly IRepository<PrioridadCatalogo> _repo;

        public PrioridadCatalogoService(IRepository<PrioridadCatalogo> repo)
        {
            _repo = repo;
        }

        public async Task<PrioridadCatalogoDto> CreateAsync(PrioridadCatalogoDto dto)
        {
            var entity = new PrioridadCatalogo
            {
                Codigo = dto.Codigo,
                Descripcion = dto.Descripcion,
                Nivel = dto.Nivel,
                SlaMultiplier = dto.SlaMultiplier,
                Icon = dto.Icon,
                Color = dto.Color,
                Activo = dto.Activo
            };
            var created = await _repo.CreateAsync(entity);
            return MapToDto(created);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _repo.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                if (ex.InnerException is PostgresException pgEx)
                {
                    if (pgEx.SqlState == "23503")
                    {
                        throw new InvalidOperationException(
                            "No se puede eliminar esta prioridad porque existen solicitudes asociadas a ella. " +
                            "Considere desactivarla en lugar de eliminarla.",
                            ex
                        );
                    }
                }
                throw;
            }
        }

        public async Task<IEnumerable<PrioridadCatalogoDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(MapToDto);
        }

        public async Task<PrioridadCatalogoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<PrioridadCatalogoDto?> UpdateAsync(int id, PrioridadCatalogoDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Codigo = dto.Codigo;
            entity.Descripcion = dto.Descripcion;
            entity.Nivel = dto.Nivel;
            entity.SlaMultiplier = dto.SlaMultiplier;
            entity.Icon = dto.Icon;
            entity.Color = dto.Color;
            entity.Activo = dto.Activo;

            await _repo.UpdateAsync(entity);
            return MapToDto(entity);
        }

        private static PrioridadCatalogoDto MapToDto(PrioridadCatalogo entity)
        {
            return new PrioridadCatalogoDto
            {
                IdPrioridad = entity.IdPrioridad,
                Codigo = entity.Codigo,
                Descripcion = entity.Descripcion,
                Nivel = entity.Nivel,
                SlaMultiplier = entity.SlaMultiplier,
                Icon = entity.Icon,
                Color = entity.Color,
                Activo = entity.Activo
            };
        }
    }
}
