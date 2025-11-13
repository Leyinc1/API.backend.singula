using System.Linq;
using Microsoft.Extensions.Logging;
using Singula.Core.Core.DTOs.Common;
using Singula.Core.Core.DTOs.Prediccion;
using Singula.Core.Core.Interfaces.Services;
using Singula.Core.Core.Interfaces;
using Singula.Core.Core.Entities;

namespace Singula.Core.Infrastructure.Services;

public class PrediccionService : IPrediccionService
{
    private readonly IPrediccionRepository _predRepo;
    private readonly ILogger<PrediccionService> _logger;

    public PrediccionService(IPrediccionRepository predRepo, ILogger<PrediccionService> logger)
    {
        _predRepo = predRepo;
        _logger = logger;
    }

    // Mock de regresión lineal simple: calcula pendiente y predice valores futuros
    public async Task<ApiResponse<PrediccionDTO>> GetByIdAsync(int id)
    {
        var p = await _predRepo.GetByIdAsync(id);
        if (p == null) return ApiResponse<PrediccionDTO>.ErrorResponse("No encontrada");
        var dto = new PrediccionDTO
        {
            Id = p.Id,
            Indicador = p.Indicador,
            ValorPredicho = p.ValorPredicho,
            FechaPrediccion = p.FechaPrediccion,
            FechaGeneracion = p.FechaGeneracion,
            Confianza = p.Confianza,
            ModeloUtilizado = p.ModeloUtilizado
        };
        return ApiResponse<PrediccionDTO>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<PagedResponse<PrediccionDTO>>> GetAllAsync(PaginationParams paginationParams)
    {
        var (items, total) = await _predRepo.GetPagedAsync(paginationParams.Page, paginationParams.PageSize);
        var dtos = items.Select(p => new PrediccionDTO
        {
            Id = p.Id,
            Indicador = p.Indicador,
            ValorPredicho = p.ValorPredicho,
            FechaPrediccion = p.FechaPrediccion,
            FechaGeneracion = p.FechaGeneracion,
            Confianza = p.Confianza,
            ModeloUtilizado = p.ModeloUtilizado
        }).ToList();

        var paged = new PagedResponse<PrediccionDTO>
        {
            Items = dtos,
            TotalItems = total,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize
        };

        return ApiResponse<PagedResponse<PrediccionDTO>>.SuccessResponse(paged);
    }

    public async Task<ApiResponse<PrediccionResultadoDTO>> GenerarPrediccionAsync(CreatePrediccionDTO dto)
    {
        // Mock: tomamos últimos N registros del repositorio de predicciones o KPI (aquí simplificado)
        var existentes = (await _predRepo.GetByIndicadorAsync(dto.Indicador)).OrderBy(p => p.FechaPrediccion).ToList();
        // Si no hay suficientes datos, devolvemos valores mock
        if (existentes.Count < 2)
        {
            var mock = new PrediccionResultadoDTO
            {
                Indicador = dto.Indicador,
                ModeloUtilizado = "Mock - Regresión Lineal Simple",
                Confianza = 0.5m
            };
            for (int i = 1; i <= dto.DiasHaciaAdelante; i++)
            {
                mock.Predicciones.Add(new PrediccionPuntoDTO { Fecha = DateTime.UtcNow.AddDays(i), ValorPredicho = 0 });
            }
            return ApiResponse<PrediccionResultadoDTO>.SuccessResponse(mock);
        }

        // Crear arrays X (tiempo) e Y (valor)
        var x = Enumerable.Range(0, existentes.Count).Select(i => (double)i).ToArray();
        var y = existentes.Select(p => (double)p.ValorPredicho).ToArray();

        // Calcular regresión lineal simple (pendiente y intercepto)
        var n = x.Length;
        var xMean = x.Average();
        var yMean = y.Average();
        var numerator = x.Zip(y, (xi, yi) => (xi - xMean) * (yi - yMean)).Sum();
        var denominator = x.Zip(x, (xi, xj) => (xi - xMean) * (xj - xMean)).Sum();
        var slope = denominator == 0 ? 0 : numerator / denominator;
        var intercept = yMean - slope * xMean;

        var resultado = new PrediccionResultadoDTO
        {
            Indicador = dto.Indicador,
            ModeloUtilizado = "Regresión Lineal Simple",
            Confianza = 0.8m
        };

        var lastIndex = x.Max();
        for (int i = 1; i <= dto.DiasHaciaAdelante; i++)
        {
            var xi = lastIndex + i;
            var yi = intercept + slope * xi;
            resultado.Predicciones.Add(new PrediccionPuntoDTO { Fecha = DateTime.UtcNow.AddDays(i), ValorPredicho = (decimal)yi });
        }

        // Guardar predicciones en DB
        foreach (var p in resultado.Predicciones)
        {
            var ent = new Prediccion
            {
                Indicador = dto.Indicador,
                ValorPredicho = p.ValorPredicho,
                FechaPrediccion = p.Fecha,
                FechaGeneracion = DateTime.UtcNow,
                Confianza = resultado.Confianza,
                ModeloUtilizado = resultado.ModeloUtilizado,
                DatosEntrada = "[]"
            };
            await _predRepo.AddAsync(ent);
        }

        return ApiResponse<PrediccionResultadoDTO>.SuccessResponse(resultado);
    }

    public async Task<ApiResponse<List<PrediccionDTO>>> GetByIndicadorAsync(string indicador)
    {
        var items = await _predRepo.GetByIndicadorAsync(indicador);
        var dtos = items.Select(p => new PrediccionDTO
        {
            Id = p.Id,
            Indicador = p.Indicador,
            ValorPredicho = p.ValorPredicho,
            FechaPrediccion = p.FechaPrediccion,
            FechaGeneracion = p.FechaGeneracion,
            Confianza = p.Confianza,
            ModeloUtilizado = p.ModeloUtilizado
        }).ToList();

        return ApiResponse<List<PrediccionDTO>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PrediccionDTO>> GetUltimaPrediccionAsync(string indicador)
    {
        var p = await _predRepo.GetUltimaPrediccionAsync(indicador);
        if (p == null) return ApiResponse<PrediccionDTO>.ErrorResponse("No encontrada");
        var dto = new PrediccionDTO
        {
            Id = p.Id,
            Indicador = p.Indicador,
            ValorPredicho = p.ValorPredicho,
            FechaPrediccion = p.FechaPrediccion,
            FechaGeneracion = p.FechaGeneracion,
            Confianza = p.Confianza,
            ModeloUtilizado = p.ModeloUtilizado
        };
        return ApiResponse<PrediccionDTO>.SuccessResponse(dto);
    }
}
