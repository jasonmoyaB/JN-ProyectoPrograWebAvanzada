using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Productos
{
    public class CrearProductoDto
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string SKU { get; set; } = string.Empty;
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Nombre { get; set; } = string.Empty;
    }
}
