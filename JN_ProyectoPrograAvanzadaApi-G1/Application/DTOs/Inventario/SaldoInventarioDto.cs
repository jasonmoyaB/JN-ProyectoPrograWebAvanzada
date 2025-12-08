namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Inventario
{
    public class SaldoInventarioDto
    {
        public int BodegaID { get; set; }
        public string BodegaNombre { get; set; } = string.Empty;
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoSKU { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal? StockMinimo { get; set; }
        public bool AlertaStockBajo { get; set; }
        public int UnidadID { get; set; }
        public string UnidadNombre { get; set; } = string.Empty;
    }
}

