

namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class SolicitudDetalle
    {
        public int SolicitudDetalleID { get; set; }
        public int SolicitudID { get; set; }
        public int ProductoID { get; set; }
        public decimal CantidadSolicitada { get; set; }
        public decimal? CantidadEnviada { get; set; }

        
        public Solicitud? Solicitud { get; set; }
        public Producto? Producto { get; set; }
    }
}

