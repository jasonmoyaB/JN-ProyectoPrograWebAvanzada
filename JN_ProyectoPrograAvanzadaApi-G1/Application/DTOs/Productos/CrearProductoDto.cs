using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Productos
{
    public class CrearProductoDto
    {
        [Required(ErrorMessage = "El SKU es obligatorio")]
        [StringLength(50, ErrorMessage = "El SKU no puede exceder 50 caracteres")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        public bool EsSerializado { get; set; } = false;
    }
}

