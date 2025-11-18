namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class MovimientoDetalleSerie
    {
        public int MovimientoDetalleSerieID { get; set; }
        public int MovimientoDetalleID { get; set; }
        public int SerieID { get; set; }

        // Navegación
        public MovimientoDetalle? MovimientoDetalle { get; set; }
        public Serie? Serie { get; set; }
    }
}

