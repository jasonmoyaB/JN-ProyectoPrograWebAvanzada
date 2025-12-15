namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{

    //Serie asociada a un detalle de movimiento
    public class MovimientoDetalleSerie
    {
        public int MovimientoDetalleSerieID { get; set; }
        public int MovimientoDetalleID { get; set; }
        public int SerieID { get; set; }

        
        public MovimientoDetalle? MovimientoDetalle { get; set; }
        public Serie? Serie { get; set; }
    }
}

