namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Solicitud
    {
        public int SolicitudID { get; set; }
        public int EstadoSolicitudID { get; set; }
        public int BodegaID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaCreacionUTC { get; set; } = DateTime.UtcNow;
        public DateTime? FechaAprobacionUTC { get; set; }
        public DateTime? FechaEnvioUTC { get; set; }
        public DateTime? FechaEntregaUTC { get; set; }
        public string? Comentarios { get; set; }
        public int? UsuarioAprobadorID { get; set; }
        public int? DespachoID { get; set; }

        // Navegación
        public EstadoSolicitud? EstadoSolicitud { get; set; }
        public Bodega? Bodega { get; set; }
        public Usuario? Usuario { get; set; }
        public Usuario? UsuarioAprobador { get; set; }
        public Despacho? Despacho { get; set; }
        public ICollection<SolicitudDetalle> Detalles { get; set; } = new List<SolicitudDetalle>();
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}

