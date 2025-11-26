using Microsoft.EntityFrameworkCore;
using Singula.Core.Infrastructure.Data;
using Singula.Core.Services.Dto;
using Singula.Core.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class SlaService : ISlaService
    {
        private readonly ApplicationDbContext _db;

        public SlaService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<SolicitudDto> CreateManualEntryAsync(ManualEntryDto dto)
        {
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var areaName = (dto.BloqueTech ?? string.Empty).Trim();
                var area = await _db.Areas.FirstOrDefaultAsync(a => a.NombreArea.ToLower() == areaName.ToLower());
                if (area == null)
                {
                    area = new Area { NombreArea = areaName, Descripcion = dto.Observaciones };
                    _db.Areas.Add(area);
                    await _db.SaveChangesAsync();
                }

                var tipoKey = (dto.TipoSolicitud ?? string.Empty).Trim();
                var tipo = await _db.TipoSolicitudCatalogos.FirstOrDefaultAsync(t => t.Codigo == tipoKey || t.Descripcion == tipoKey);
                if (tipo == null)
                {
                    tipo = new TipoSolicitudCatalogo { Codigo = string.IsNullOrWhiteSpace(tipoKey) ? $"TS_{Guid.NewGuid():N}" : tipoKey.Replace(" ", "_").ToUpper(), Descripcion = dto.TipoSolicitud };
                    _db.TipoSolicitudCatalogos.Add(tipo);
                    await _db.SaveChangesAsync();
                }

                var rol = await _db.RolRegistros.FirstOrDefaultAsync(r => (r.BloqueTech != null && r.BloqueTech.ToLower() == areaName.ToLower()) || (r.NombreRol != null && r.NombreRol.ToLower() == areaName.ToLower()));
                if (rol == null)
                {
                    rol = new RolRegistro { BloqueTech = areaName, NombreRol = areaName, Descripcion = dto.Observaciones, EsActivo = true };
                    _db.RolRegistros.Add(rol);
                    await _db.SaveChangesAsync();
                }

                var config = await _db.ConfigSlas.FirstOrDefaultAsync(c => c.IdTipoSolicitud == tipo.IdTipoSolicitud);
                if (config == null)
                {
                    var dias = (!string.IsNullOrWhiteSpace(dto.TipoSolicitud) && dto.TipoSolicitud.ToLower().Contains("reempl")) ? 20 : 35;
                    config = new ConfigSla { CodigoSla = (tipo.Codigo ?? $"SLA_{Guid.NewGuid():N}").ToUpper(), Descripcion = $"SLA automático para {tipo.Descripcion}", DiasUmbral = dias, EsActivo = true, IdTipoSolicitud = tipo.IdTipoSolicitud, CreadoEn = DateTime.UtcNow };
                    _db.ConfigSlas.Add(config);
                    await _db.SaveChangesAsync();
                }

                var estado = await _db.EstadoSolicitudCatalogos.FirstOrDefaultAsync();
                if (estado == null)
                {
                    estado = new EstadoSolicitudCatalogo { Codigo = "ING", Descripcion = "Ingresado" };
                    _db.EstadoSolicitudCatalogos.Add(estado);
                    await _db.SaveChangesAsync();
                }

                Usuario createdByUser = null;
                if (dto.CreadoPor.HasValue)
                {
                    createdByUser = await _db.Usuarios.FindAsync(dto.CreadoPor.Value);
                }
                if (createdByUser == null)
                {
                    createdByUser = await _db.Usuarios.FirstOrDefaultAsync();
                }
                if (createdByUser == null)
                {
                    var rolSistema = await _db.RolesSistemas.FirstOrDefaultAsync();
                    var estadoUsuario = await _db.EstadoUsuarioCatalogos.FirstOrDefaultAsync();
                    var sysUser = new Usuario { Username = "system", Correo = $"system+{Guid.NewGuid():N}@local", PasswordHash = null, IdRolSistema = rolSistema?.IdRolSistema ?? 1, IdEstadoUsuario = estadoUsuario?.IdEstadoUsuario ?? 1, CreadoEn = DateTime.UtcNow };
                    _db.Usuarios.Add(sysUser);
                    await _db.SaveChangesAsync();
                    createdByUser = sysUser;
                }

                var personalUser = new Usuario { Username = string.IsNullOrWhiteSpace(dto.NombrePersonal) ? $"u_{Guid.NewGuid():N}" : dto.NombrePersonal.Replace(" ", "_").ToLower(), Correo = $"no-reply+{Guid.NewGuid():N}@local", PasswordHash = null, IdRolSistema = createdByUser.IdRolSistema, IdEstadoUsuario = createdByUser.IdEstadoUsuario, CreadoEn = DateTime.UtcNow };
                _db.Usuarios.Add(personalUser);
                await _db.SaveChangesAsync();

                var personal = new Personal { IdUsuario = personalUser.IdUsuario, Nombres = dto.NombrePersonal, Apellidos = null, Documento = null, Estado = null, CreadoEn = DateTime.UtcNow };
                _db.Personals.Add(personal);
                await _db.SaveChangesAsync();

                int? numDias = null;
                if (dto.FechaSolicitud.HasValue && dto.FechaIngreso.HasValue)
                {
                    numDias = (int)(dto.FechaIngreso.Value.Date - dto.FechaSolicitud.Value.Date).TotalDays;
                    if (numDias < 0) numDias = 0;
                }

                var solicitud = new Solicitud { IdPersonal = personal.IdPersonal, IdRolRegistro = rol.IdRolRegistro, IdSla = config.IdSla, IdArea = area.IdArea, IdEstadoSolicitud = estado.IdEstadoSolicitud, FechaSolicitud = dto.FechaSolicitud, FechaIngreso = dto.FechaIngreso, NumDiasSla = numDias, ResumenSla = dto.Observaciones, OrigenDato = dto.CreadoManualmente ? "manual" : "import", Prioridad = dto.Prioridad, CreadoPor = createdByUser.IdUsuario, CreadoEn = DateTime.UtcNow };
                _db.Solicituds.Add(solicitud);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                return new SolicitudDto { IdSolicitud = solicitud.IdSolicitud, IdPersonal = solicitud.IdPersonal, IdRolRegistro = solicitud.IdRolRegistro, IdSla = solicitud.IdSla, IdArea = solicitud.IdArea, IdEstadoSolicitud = solicitud.IdEstadoSolicitud, FechaSolicitud = solicitud.FechaSolicitud, FechaIngreso = solicitud.FechaIngreso, NumDiasSla = solicitud.NumDiasSla, ResumenSla = solicitud.ResumenSla, OrigenDato = solicitud.OrigenDato, Prioridad = solicitud.Prioridad, CreadoPor = solicitud.CreadoPor, CreadoEn = solicitud.CreadoEn };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<UploadResultDto> SaveUploadedFileAsync(byte[] fileBytes, string fileName)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"upload_{Guid.NewGuid():N}_{fileName}");
            await File.WriteAllBytesAsync(tempFile, fileBytes);
            return new UploadResultDto { FileName = fileName, Length = fileBytes.Length, TempPath = tempFile };
        }

        public async Task<IEnumerable<SolicitudDto>> ImportBatchAsync(IEnumerable<ManualEntryDto> rows, int? creadoPor = null)
        {
            var results = new List<SolicitudDto>();
            foreach (var row in rows)
            {
                var dto = row;
                if (creadoPor.HasValue) dto.CreadoPor = creadoPor;
                var created = await CreateManualEntryAsync(dto);
                results.Add(created);
            }
            return results;
        }
    }
}
