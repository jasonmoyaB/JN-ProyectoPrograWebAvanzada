


namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Dashboard
{
    public class KpiRotacionDto
    {
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoSKU { get; set; } = string.Empty;
        public int BodegaID { get; set; }
        public string BodegaNombre { get; set; } = string.Empty;
        public decimal CantidadSalida { get; set; }
        public decimal StockPromedio { get; set; }
        public decimal Rotacion { get; set; } 
        public int DiasAnalizados { get; set; }
    }
}

