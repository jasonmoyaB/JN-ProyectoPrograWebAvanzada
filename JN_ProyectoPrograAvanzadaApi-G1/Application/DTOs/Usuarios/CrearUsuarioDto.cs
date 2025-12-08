using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaApi_G1.Application.DTOs.Usuarios
{
    public class CrearUsuarioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder 100 caracteres")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RolID { get; set; }

        public int? BodegaID { get; set; }
    }
}

