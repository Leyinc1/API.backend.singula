using Microsoft.Extensions.Logging;
using Singula.Core.Core.Interfaces.Services;

namespace Singula.Core.Infrastructure.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly ILogger<PdfGeneratorService> _logger;

    public PdfGeneratorService(ILogger<PdfGeneratorService> logger)
    {
        _logger = logger;
    }

    public Task<byte[]> GenerarReporteKPIAsync(object data, string titulo)
    {
        _logger.LogInformation("Generando PDF de KPI: {titulo}", titulo);
        // Mock: retornar PDF vacío
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<byte[]> GenerarReporteComparativoAsync(object data, string titulo)
    {
        _logger.LogInformation("Generando PDF comparativo: {titulo}", titulo);
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<byte[]> GenerarReporteTendenciasAsync(object data, string titulo)
    {
        _logger.LogInformation("Generando PDF de tendencias: {titulo}", titulo);
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<string> GuardarPdfAsync(byte[] pdfBytes, string nombreArchivo)
    {
        // Guardado temporal no implementado en este servicio
        _logger.LogInformation("GuardarPdf: {nombreArchivo} tamaño {size}", nombreArchivo, pdfBytes.Length);
        return Task.FromResult(string.Empty);
    }
}
