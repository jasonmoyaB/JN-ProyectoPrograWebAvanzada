namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class EstadoSolicitud
    {
        public int EstadoSolicitudID { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Nombre { get; set; }

        // Navegación
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    }
}

