using System.Linq;
using Microsoft.Extensions.Logging;
using Singula.Core.Core.DTOs.ArchivoExcel;
using Singula.Core.Core.Interfaces.Services;
using Singula.Core.Core.Interfaces;
using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Services;

public class ArchivoExcelService : IArchivoExcelService
{
    private readonly IUnitOfWork _uow;
    private readonly IFileStorageService _storage;
    private readonly IExcelProcessorService _excelProcessor;
    private readonly ILogger<ArchivoExcelService> _logger;

    public ArchivoExcelService(IUnitOfWork uow, IFileStorageService storage, IExcelProcessorService excelProcessor, ILogger<ArchivoExcelService> logger)
    {
        _uow = uow;
        _storage = storage;
        _excelProcessor = excelProcessor;
        _logger = logger;
    }

    public async Task<ApiResponse<ArchivoExcelDTO>> GetByIdAsync(int id)
    {
        var e = await _uow.ArchivoExcel.GetByIdAsync(id);
        if (e == null) return ApiResponse<ArchivoExcelDTO>.ErrorResponse("No encontrado");
        return ApiResponse<ArchivoExcelDTO>.SuccessResponse(MapToDto(e));
    }

    public async Task<ApiResponse<PagedResponse<ArchivoExcelDTO>>> GetAllAsync(PaginationParams paginationParams)
    {
        var (items, total) = await _uow.ArchivoExcel.GetPagedAsync(paginationParams.Page, paginationParams.PageSize, null, x => x.FechaCarga, false);
        var dtos = items.Select(MapToDto).ToList();
        var paged = new PagedResponse<ArchivoExcelDTO> { Items = dtos, TotalItems = total, Page = paginationParams.Page, PageSize = paginationParams.PageSize };
        return ApiResponse<PagedResponse<ArchivoExcelDTO>>.SuccessResponse(paged);
    }

    public async Task<ApiResponse<ArchivoExcelDTO>> CreateAsync(CreateArchivoExcelDTO dto)
    {
        var e = new ArchivoExcel
        {
            NombreArchivo = dto.NombreArchivo,
            TamanoBytes = dto.TamanoBytes,
            UsuarioCarga = dto.UsuarioCarga,
            FechaCarga = DateTime.UtcNow,
            Estado = EstadoProcesamiento.Pendiente
        };
        await _uow.ArchivoExcel.AddAsync(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<ArchivoExcelDTO>.SuccessResponse(MapToDto(e));
    }

    public async Task<ApiResponse<ArchivoExcelDTO>> UpdateAsync(int id, UpdateArchivoExcelDTO dto)
    {
        var e = await _uow.ArchivoExcel.GetByIdAsync(id);
        if (e == null) return ApiResponse<ArchivoExcelDTO>.ErrorResponse("No encontrado");
        e.Estado = dto.Estado;
        e.MensajeError = dto.MensajeError;
        e.UpdatedAt = DateTime.UtcNow;
        await _uow.ArchivoExcel.UpdateAsync(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<ArchivoExcelDTO>.SuccessResponse(MapToDto(e));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var e = await _uow.ArchivoExcel.GetByIdAsync(id);
        if (e == null) return ApiResponse<bool>.ErrorResponse("No encontrado");
        await _uow.ArchivoExcel.DeleteAsync(e);
        await _uow.SaveChangesAsync();
        return ApiResponse<bool>.SuccessResponse(true);
    }

    public async Task<ApiResponse<ArchivoExcelDTO>> ProcesarArchivoAsync(string rutaArchivo, int archivoId)
    {
        var archivo = await _uow.ArchivoExcel.GetByIdAsync(archivoId);
        if (archivo == null) return ApiResponse<ArchivoExcelDTO>.ErrorResponse("Archivo no encontrado");

        archivo.Estado = EstadoProcesamiento.Procesando;
        await _uow.ArchivoExcel.UpdateAsync(archivo);
        await _uow.SaveChangesAsync();

        try
        {
            var registros = await _excelProcessor.LeerArchivoExcelAsync(rutaArchivo);
            foreach (var r in registros)
            {
                var ent = new RegistroKPI
                {
                    ArchivoExcelId = archivoId,
                    Indicador = r.Indicador,
                    ValorActual = r.ValorActual,
                    ValorMeta = r.ValorMeta,
                    PorcentajeCumplimiento = r.ValorMeta == 0 ? 0 : Math.Round(r.ValorActual / r.ValorMeta * 100, 2),
                    FechaMedicion = r.FechaMedicion,
                    Area = r.Area,
                    Responsable = r.Responsable,
                    Observaciones = r.Observaciones
                };
                await _uow.RegistroKPI.AddAsync(ent);
            }

            archivo.Estado = EstadoProcesamiento.Completado;
            await _uow.ArchivoExcel.UpdateAsync(archivo);
            await _uow.SaveChangesAsync();

            return ApiResponse<ArchivoExcelDTO>.SuccessResponse(MapToDto(archivo));
        }
        catch (Exception ex)
        {
            archivo.Estado = EstadoProcesamiento.Error;
            archivo.MensajeError = ex.Message;
            await _uow.ArchivoExcel.UpdateAsync(archivo);
            await _uow.SaveChangesAsync();
            _logger.LogError(ex, "Error al procesar archivo {id}", archivoId);
            return ApiResponse<ArchivoExcelDTO>.ErrorResponse("Error al procesar archivo");
        }
    }

    public async Task<ApiResponse<List<ArchivoExcelDTO>>> GetRecentesAsync(int cantidad = 10)
    {
        var list = await _uow.ArchivoExcel.GetRecentesAsync(cantidad);
        return ApiResponse<List<ArchivoExcelDTO>>.SuccessResponse(list.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<List<ArchivoExcelDTO>>> GetByEstadoAsync(EstadoProcesamiento estado)
    {
        var list = await _uow.ArchivoExcel.GetByEstadoAsync(estado);
        return ApiResponse<List<ArchivoExcelDTO>>.SuccessResponse(list.Select(MapToDto).ToList());
    }

    private ArchivoExcelDTO MapToDto(ArchivoExcel e)
    {
        return new ArchivoExcelDTO
        {
            Id = e.Id,
            NombreArchivo = e.NombreArchivo,
            RutaArchivo = e.RutaArchivo,
            TamanoBytes = e.TamanoBytes,
            FechaCarga = e.FechaCarga,
            UsuarioCarga = e.UsuarioCarga,
            Estado = e.Estado,
            MensajeError = e.MensajeError,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            IsActive = e.IsActive,
            CantidadRegistros = e.Registros?.Count ?? 0
        };
    }
}
