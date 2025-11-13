using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text(titulo).SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);
                page.Content().Column(column =>
                {
                    column.Item().Text("Reporte de KPIs");
                    column.Item().Text(DateTime.UtcNow.ToString("G"));

                    column.Item().Element(c =>
                    {
                        c.Text("Datos:");
                        c.Text(data?.ToString() ?? "Sin datos");
                    });
                });
                page.Footer().AlignCenter().Text("Página");
            });
        });

        using var ms = new MemoryStream();
        doc.GeneratePdf(ms);
        return Task.FromResult(ms.ToArray());
    }

    public Task<byte[]> GenerarReporteComparativoAsync(object data, string titulo)
    {
        return GenerarReporteKPIAsync(data, titulo);
    }

    public Task<byte[]> GenerarReporteTendenciasAsync(object data, string titulo)
    {
        return GenerarReporteKPIAsync(data, titulo);
    }

    public async Task<string> GuardarPdfAsync(byte[] pdfBytes, string nombreArchivo)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, nombreArchivo);
        await File.WriteAllBytesAsync(filePath, pdfBytes);
        return filePath;
    }
}
