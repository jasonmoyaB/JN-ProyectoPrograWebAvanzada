namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Dashboard
{
    public class KpiProductosSolicitadosDto
    {
        public int ProductoID { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string ProductoSKU { get; set; } = string.Empty;
        public decimal TotalSolicitado { get; set; }
        public int CantidadSolicitudes { get; set; }
    }
}

