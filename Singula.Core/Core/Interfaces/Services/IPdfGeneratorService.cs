namespace Singula.Core.Core.Interfaces.Services;

/// <summary>
/// Servicio para generar reportes PDF
/// </summary>
public interface IPdfGeneratorService
{
    Task<byte[]> GenerarReporteKPIAsync(object data, string titulo);
    Task<byte[]> GenerarReporteComparativoAsync(object data, string titulo);
    Task<byte[]> GenerarReporteTendenciasAsync(object data, string titulo);
    Task<string> GuardarPdfAsync(byte[] pdfBytes, string nombreArchivo);
}
