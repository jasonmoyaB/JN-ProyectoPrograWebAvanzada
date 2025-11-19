using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Contrasena { get; set; } = string.Empty;
    }
}

