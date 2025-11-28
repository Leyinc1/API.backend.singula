using ClosedXML.Excel;

var filePath = @"c:\Users\PC\Documents\WEB\API.backend.singula\API.backend.singula\API.backend.singula\Uploads\uploads\sla\20251128045657932_7cefb4d1c2114d37a4c84de011044b70_registros_sla_generado1s.xlsx";

Console.WriteLine($"Leyendo archivo: {filePath}");
Console.WriteLine();

using var workbook = new XLWorkbook(filePath);
var worksheet = workbook.Worksheets.First();

Console.WriteLine("=== ENCABEZADOS DEL EXCEL ===");
foreach (var cell in worksheet.Row(1).CellsUsed())
{
    Console.WriteLine($"Columna {cell.Address.ColumnNumber}: '{cell.GetString()}'");
}

Console.WriteLine();
Console.WriteLine("=== PRIMERA FILA DE DATOS ===");
foreach (var cell in worksheet.Row(2).CellsUsed())
{
    Console.WriteLine($"Columna {cell.Address.ColumnNumber}: '{cell.GetString()}'");
}
