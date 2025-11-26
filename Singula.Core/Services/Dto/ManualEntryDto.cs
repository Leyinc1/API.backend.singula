using System;

namespace Singula.Core.Services.Dto
{
    public class ManualEntryDto
    {
        public string BloqueTech { get; set; }
        public string TipoSolicitud { get; set; }
        public string Prioridad { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string NombrePersonal { get; set; }
        public string Observaciones { get; set; }
        public bool CreadoManualmente { get; set; }
        public int? CreadoPor { get; set; }
    }
}
