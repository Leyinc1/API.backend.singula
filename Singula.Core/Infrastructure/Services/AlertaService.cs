using System.Linq;
using Microsoft.Extensions.Logging;
using Singula.Core.Core.DTOs.Alerta;
using Singula.Core.Core.Interfaces.Services;
using Singula.Core.Core.Interfaces;
using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Services;

public class AlertaService : IAlertaService
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly ILogger<AlertaService> _logger;

    public AlertaService(IUnitOfWork uow, IEmailService emailService, ILogger<AlertaService> logger)
    {
        _uow = uow;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ApiResponse<AlertaDTO>> GetByIdAsync(int id)
    {
        var e = await _uow.Alerta.GetByIdAsync(id);
        if (e == null) return ApiResponse<AlertaDTO>.ErrorResponse("No encontrada");
        return ApiResponse<AlertaDTO>.SuccessResponse(MapToDto(e));
    }

    public async Task<ApiResponse<PagedResponse<AlertaDTO>>> GetAllAsync(PaginationParams paginationParams)
    {
        var (items, total) = await _uow.Alerta.GetPagedAsync(paginationParams.Page, paginationParams.PageSize);
        var dtos = items.Select(MapToDto).ToList();
        var paged = new PagedResponse<AlertaDTO> { Items = dtos, TotalItems = total, Page = paginationParams.Page, PageSize = paginationParams.PageSize };
        return ApiResponse<PagedResponse<AlertaDTO>>.SuccessResponse(paged);
    }

    public async Task<ApiResponse<AlertaDTO>> CreateAsync(CreateAlertaDTO dto)
    {
        var e = new Alerta
        {
            RegistroKPIId = dto.RegistroKPIId,
            Tipo = dto.Tipo,
            Nivel = dto.Nivel,
            Mensaje = dto.Mensaje,
            FechaGeneracion = DateTime.UtcNow
        };
        await _uow.Alerta.AddAsync(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<AlertaDTO>.SuccessResponse(MapToDto(e));
    }

    public async Task<ApiResponse<AlertaDTO>> UpdateAsync(int id, UpdateAlertaDTO dto)
    {
        var e = await _uow.Alerta.GetByIdAsync(id);
        if (e == null) return ApiResponse<AlertaDTO>.ErrorResponse("No encontrada");
        e.Leida = dto.Leida ?? e.Leida;
        e.EnviadaPorEmail = dto.EnviadaPorEmail ?? e.EnviadaPorEmail;
        await _uow.Alerta.UpdateAsync(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<AlertaDTO>.SuccessResponse(MapToDto(e));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _uow.Alerta.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.ErrorResponse("No encontrada");
        await _uow.Alerta.DeleteAsync(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<List<AlertaDTO>>> GetNoLeidasAsync()
    {
        var list = await _uow.Alerta.GetNoLeidasAsync();
        return ApiResponse<List<AlertaDTO>>.SuccessResponse(list.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<AlertaResumenDTO>> GetResumenAsync()
    {
        var total = await _uow.Alerta.CountAsync();
        var noLeidas = await _uow.Alerta.GetCountNoLeidasAsync();
        var criticas = (await _uow.Alerta.GetByNivelAsync(NivelSeveridad.Critico)).Count();
        var ultimas = (await _uow.Alerta.GetNoLeidasAsync()).Take(10).Select(MapToDto).ToList();
        var resumen = new AlertaResumenDTO { TotalAlertas = total, AlertasNoLeidas = noLeidas, AlertasCriticas = criticas, UltimasAlertas = ultimas };
        return ApiResponse<AlertaResumenDTO>.SuccessResponse(resumen);
    }

    public async Task<ApiResponse<bool>> MarcarComoLeidaAsync(int id)
    {
        await _uow.Alerta.MarcarComoLeidaAsync(id);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<bool>> EnviarAlertaPorEmailAsync(int id)
    {
        var e = await _uow.Alerta.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.ErrorResponse("No encontrada");
        // mock
        await _emailService.EnviarAlertaAsync("dest@example.com", "Alerta: " + e.Mensaje, e.Mensaje);
        await _uow.Alerta.MarcarComoEnviadaAsync(id);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task GenerarAlertasAutomaticasAsync()
    {
        // Lógica simple: generar alerta si porcentaje de cumplimiento < 70
        var registros = await _uow.RegistroKPI.FindAsync(r => r.PorcentajeCumplimiento < 70);
        foreach (var reg in registros)
        {
            var alerta = new Alerta
            {
                RegistroKPIId = reg.Id,
                Tipo = TipoAlerta.BajoCumplimiento,
                Nivel = reg.PorcentajeCumplimiento < 50 ? NivelSeveridad.Critico : NivelSeveridad.Advertencia,
                Mensaje = $"Indicador {reg.Indicador} con {reg.PorcentajeCumplimiento}%",
                FechaGeneracion = DateTime.UtcNow
            };
            await _uow.Alerta.AddAsync(alerta);
        }
        await _uow.SaveChangesAsync();
    }

    private AlertaDTO MapToDto(Alerta e)
    {
        return new AlertaDTO
        {
            Id = e.Id,
            RegistroKPIId = e.RegistroKPIId,
            Tipo = e.Tipo,
            Nivel = e.Nivel,
            Mensaje = e.Mensaje,
            FechaGeneracion = e.FechaGeneracion,
            Leida = e.Leida,
            EnviadaPorEmail = e.EnviadaPorEmail,
            FechaEnvioEmail = e.FechaEnvioEmail,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            IsActive = e.IsActive
        };
    }
}
