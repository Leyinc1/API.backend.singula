using System.Linq;
using Microsoft.Extensions.Logging;
using Singula.Core.Core.DTOs.RegistroKPI;
using Singula.Core.Core.Interfaces.Services;
using Singula.Core.Core.Interfaces;
using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Services;

public class RegistroKPIService : IRegistroKPIService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<RegistroKPIService> _logger;

    public RegistroKPIService(IUnitOfWork uow, ILogger<RegistroKPIService> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ApiResponse<RegistroKPIDTO>> GetByIdAsync(int id)
    {
        var entity = await _uow.RegistroKPI.GetByIdAsync(id);
        if (entity == null) return ApiResponse<RegistroKPIDTO>.ErrorResponse("No encontrado");
        var dto = MapToDto(entity);
        return ApiResponse<RegistroKPIDTO>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<PagedResponse<RegistroKPIDTO>>> GetAllAsync(PaginationParams paginationParams)
    {
        var (items, total) = await _uow.RegistroKPI.GetPagedAsync(paginationParams.Page, paginationParams.PageSize);
        var dtos = items.Select(MapToDto).ToList();
        var paged = new PagedResponse<RegistroKPIDTO> { Items = dtos, TotalItems = total, Page = paginationParams.Page, PageSize = paginationParams.PageSize };
        return ApiResponse<PagedResponse<RegistroKPIDTO>>.SuccessResponse(paged);
    }

    public async Task<ApiResponse<RegistroKPIDTO>> CreateAsync(CreateRegistroKPIDTO dto)
    {
        var entity = new RegistroKPI
        {
            ArchivoExcelId = dto.ArchivoExcelId,
            Indicador = dto.Indicador,
            ValorActual = dto.ValorActual,
            ValorMeta = dto.ValorMeta,
            PorcentajeCumplimiento = dto.ValorMeta == 0 ? 0 : Math.Round(dto.ValorActual / dto.ValorMeta * 100, 2),
            FechaMedicion = dto.FechaMedicion,
            Area = dto.Area,
            Responsable = dto.Responsable,
            Observaciones = dto.Observaciones
        };

        await _uow.RegistroKPI.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return ApiResponse<RegistroKPIDTO>.SuccessResponse(MapToDto(entity));
    }

    public async Task<ApiResponse<RegistroKPIDTO>> UpdateAsync(int id, UpdateRegistroKPIDTO dto)
    {
        var entity = await _uow.RegistroKPI.GetByIdAsync(id);
        if (entity == null) return ApiResponse<RegistroKPIDTO>.ErrorResponse("No encontrado");

        entity.Indicador = dto.Indicador ?? entity.Indicador;
        entity.ValorActual = dto.ValorActual ?? entity.ValorActual;
        entity.ValorMeta = dto.ValorMeta ?? entity.ValorMeta;
        entity.FechaMedicion = dto.FechaMedicion ?? entity.FechaMedicion;
        entity.Area = dto.Area ?? entity.Area;
        entity.Responsable = dto.Responsable ?? entity.Responsable;
        entity.Observaciones = dto.Observaciones ?? entity.Observaciones;
        entity.PorcentajeCumplimiento = entity.ValorMeta == 0 ? 0 : Math.Round(entity.ValorActual / entity.ValorMeta * 100, 2);

        await _uow.RegistroKPI.UpdateAsync(entity);
        await _uow.SaveChangesAsync();

        return ApiResponse<RegistroKPIDTO>.SuccessResponse(MapToDto(entity));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _uow.RegistroKPI.GetByIdAsync(id);
        if (entity == null) return ApiResponse<bool>.ErrorResponse("No encontrado");
        await _uow.RegistroKPI.DeleteAsync(entity);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<List<RegistroKPIDTO>>> GetByIndicadorAsync(string indicador)
    {
        var list = await _uow.RegistroKPI.GetByIndicadorAsync(indicador);
        return ApiResponse<List<RegistroKPIDTO>>.SuccessResponse(list.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<List<RegistroKPIDTO>>> GetByAreaAsync(string area)
    {
        var list = await _uow.RegistroKPI.GetByAreaAsync(area);
        return ApiResponse<List<RegistroKPIDTO>>.SuccessResponse(list.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<List<KPIDashboardDTO>>> GetDashboardDataAsync()
    {
        var indicadores = await _uow.RegistroKPI.GetIndicadoresUnicosAsync();
        var result = new List<KPIDashboardDTO>();
        foreach (var ind in indicadores)
        {
            var ultimo = (await _uow.RegistroKPI.GetByIndicadorAsync(ind)).OrderByDescending(x => x.FechaMedicion).FirstOrDefault();
            if (ultimo == null) continue;
            var historico = (await _uow.RegistroKPI.GetByIndicadorAsync(ind)).OrderBy(x => x.FechaMedicion).Select(x => new KPIHistoricoDTO { Fecha = x.FechaMedicion, Valor = x.ValorActual }).ToList();
            result.Add(new KPIDashboardDTO
            {
                Indicador = ind,
                UltimoValor = ultimo.ValorActual,
                Meta = ultimo.ValorMeta,
                PorcentajeCumplimiento = ultimo.PorcentajeCumplimiento,
                Tendencia = "Estable",
                Historico = historico
            });
        }
        return ApiResponse<List<KPIDashboardDTO>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<KPIDashboardDTO>> GetKPIHistoricoAsync(string indicador, DateTime? desde = null, DateTime? hasta = null)
    {
        var desdeVal = desde ?? DateTime.UtcNow.AddMonths(-3);
        var hastaVal = hasta ?? DateTime.UtcNow;
        var registros = await _uow.RegistroKPI.GetByFechaRangoAsync(desdeVal, hastaVal);
        registros = registros.Where(r => r.Indicador == indicador).OrderBy(r => r.FechaMedicion);
        var ultimo = registros.LastOrDefault();
        if (ultimo == null) return ApiResponse<KPIDashboardDTO>.ErrorResponse("No hay datos");
        var historico = registros.Select(x => new KPIHistoricoDTO { Fecha = x.FechaMedicion, Valor = x.ValorActual }).ToList();
        var dto = new KPIDashboardDTO
        {
            Indicador = indicador,
            UltimoValor = ultimo.ValorActual,
            Meta = ultimo.ValorMeta,
            PorcentajeCumplimiento = ultimo.PorcentajeCumplimiento,
            Tendencia = "Estable",
            Historico = historico
        };
        return ApiResponse<KPIDashboardDTO>.SuccessResponse(dto);
    }

    private RegistroKPIDTO MapToDto(RegistroKPI e)
    {
        return new RegistroKPIDTO
        {
            Id = e.Id,
            ArchivoExcelId = e.ArchivoExcelId,
            Indicador = e.Indicador,
            ValorActual = e.ValorActual,
            ValorMeta = e.ValorMeta,
            PorcentajeCumplimiento = e.PorcentajeCumplimiento,
            FechaMedicion = e.FechaMedicion,
            Area = e.Area,
            Responsable = e.Responsable,
            Observaciones = e.Observaciones,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            IsActive = e.IsActive
        };
    }
}
