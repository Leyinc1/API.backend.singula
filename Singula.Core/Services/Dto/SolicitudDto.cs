using System;

namespace Singula.Core.Services.Dto
{
    public class SolicitudDto
    {
        public int IdSolicitud { get; set; }
        public int IdPersonal { get; set; }
        public int IdRolRegistro { get; set; }
        public int IdSla { get; set; }
        public int IdArea { get; set; }
        public int IdEstadoSolicitud { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? NumDiasSla { get; set; }
        public string? ResumenSla { get; set; }
        public string? OrigenDato { get; set; }
        public string? Prioridad { get; set; }
        public int CreadoPor { get; set; }
        public DateTime? CreadoEn { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public int? ActualizadoPor { get; set; }
    }
}
