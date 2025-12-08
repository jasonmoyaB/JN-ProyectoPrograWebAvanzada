namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class MovimientoDetalleSerie
    {
        public int MovimientoDetalleSerieID { get; set; }
        public int MovimientoDetalleID { get; set; }
        public int SerieID { get; set; }

        
        public MovimientoDetalle? MovimientoDetalle { get; set; }
        public Serie? Serie { get; set; }
    }
}

