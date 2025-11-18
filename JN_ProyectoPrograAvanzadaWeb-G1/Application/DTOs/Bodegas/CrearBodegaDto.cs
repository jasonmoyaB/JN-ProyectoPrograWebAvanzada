using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Bodegas
{
    public class CrearBodegaDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        public string? Ubicacion { get; set; }
    }
}

