namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Despacho
    {
        public int DespachoID { get; set; }
        public int? SolicitudID { get; set; }
        public string NumeroEnvio { get; set; } = string.Empty;
        public int BodegaOrigenID { get; set; }
        public int BodegaDestinoID { get; set; }
        public string Estado { get; set; } = string.Empty; // Pendiente, Enviado, Recibido
        public int UsuarioCreadorID { get; set; }
        public DateTime FechaCreacionUTC { get; set; } = DateTime.UtcNow;
        public DateTime? FechaDespachoUTC { get; set; }
        public DateTime? FechaRecepcionUTC { get; set; }

        // Navegación
        public Solicitud? Solicitud { get; set; }
        public Bodega? BodegaOrigen { get; set; }
        public Bodega? BodegaDestino { get; set; }
        public Usuario? UsuarioCreador { get; set; }
        public ICollection<DespachoDetalle> Detalles { get; set; } = new List<DespachoDetalle>();
    }
}

