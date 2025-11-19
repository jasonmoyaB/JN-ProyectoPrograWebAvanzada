namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool EsSerializado { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime? FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
