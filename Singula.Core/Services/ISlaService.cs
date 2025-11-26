using Singula.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Singula.Core.Services
{
    public interface ISlaService
    {
        Task<SolicitudDto> CreateManualEntryAsync(ManualEntryDto dto);
        Task<UploadResultDto> SaveUploadedFileAsync(byte[] fileBytes, string fileName);
        Task<IEnumerable<SolicitudDto>> ImportBatchAsync(IEnumerable<ManualEntryDto> rows, int? creadoPor = null);
    }
}
