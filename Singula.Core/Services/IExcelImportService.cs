using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public class ExcelImportResult
    {
        public bool Success { get; set; }
        public int TotalRows { get; set; }
        public int ImportedRows { get; set; }
        public int FailedRows { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
    }

    public interface IExcelImportService
    {
        /// <summary>
        /// Importa datos desde un archivo Excel a la base de datos
        /// </summary>
        /// <param name="filePath">Ruta completa del archivo Excel</param>
        /// <returns>Resultado de la importación</returns>
        Task<ExcelImportResult> ImportFromExcelAsync(string filePath);
    }
}
