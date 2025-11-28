namespace Singula.Core.Core.DTOs
{
    public class AlertaDashboardDto
    {
        public int IdAlerta { get; set; }
        public string Mensaje { get; set; }
        public string Nivel { get; set; }
        public string RolAfectado { get; set; }
        public int DiasTotal { get; set; }
        public int DiasLimite { get; set; }
        public string TipoAlerta { get; set; }
        public int IdTipoAlerta { get; set; }
        public bool EsNueva { get; set; }
        public DateTime? FechaSolicitud { get; set; }

        // --- NUEVO CAMPO ---
        public DateTime? FechaCreacionSla { get; set; }
    }
}