using System.Linq;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Singula.Core.Core.DTOs.RegistroKPI;
using Singula.Core.Core.Interfaces.Services;

namespace Singula.Core.Infrastructure.Services;

public class ExcelProcessorService : IExcelProcessorService
{
    private readonly ILogger<ExcelProcessorService> _logger;

    public ExcelProcessorService(ILogger<ExcelProcessorService> logger)
    {
        _logger = logger;
        // EPPlus requires noncommercial license configuration when used in production.
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<bool> ValidarFormatoAsync(string rutaArchivo)
    {
        // Implementación simple: verificar extensión
        var ext = Path.GetExtension(rutaArchivo)?.ToLowerInvariant();
        return ext == ".xlsx" || ext == ".xls";
    }

    public async Task<List<CreateRegistroKPIDTO>> LeerArchivoExcelAsync(string rutaArchivo)
    {
        var result = new List<CreateRegistroKPIDTO>();
        if (!File.Exists(rutaArchivo)) return result;

        using var package = new ExcelPackage(new FileInfo(rutaArchivo));
        var ws = package.Workbook.Worksheets.FirstOrDefault();
        if (ws == null) return result;

        // Suponemos que la primera fila es encabezado y los campos están en columnas definidas
        // Indicador | ValorActual | ValorMeta | FechaMedicion | Area | Responsable | Observaciones
        var startRow = 2;
        for (int row = startRow; row <= ws.Dimension.End.Row; row++)
        {
            try
            {
                var indicador = ws.Cells[row, 1].GetValue<string>() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(indicador)) continue;

                var valorActual = ws.Cells[row, 2].GetValue<decimal>();
                var valorMeta = ws.Cells[row, 3].GetValue<decimal>();
                var fechaMedicion = ws.Cells[row, 4].GetValue<DateTime>();
                var area = ws.Cells[row, 5].GetValue<string>();
                var responsable = ws.Cells[row, 6].GetValue<string>();
                var observaciones = ws.Cells[row, 7].GetValue<string>();

                result.Add(new CreateRegistroKPIDTO
                {
                    Indicador = indicador,
                    ValorActual = valorActual,
                    ValorMeta = valorMeta,
                    FechaMedicion = fechaMedicion,
                    Area = area,
                    Responsable = responsable,
                    Observaciones = observaciones
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error procesando fila {row}", row);
            }
        }

        return result;
    }

    public Task<byte[]> ExportarAExcelAsync<T>(List<T> data, string nombreHoja = "Datos")
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add(nombreHoja);
        // Implementación simple: no reflection, solo headers si T tiene propiedades.
        // Por ahora retornamos vacío
        return Task.FromResult(package.GetAsByteArray());
    }
}
