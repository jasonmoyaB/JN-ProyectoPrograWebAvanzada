using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Application.DTOs.Usuarios
{
    public class EditarUsuarioDto
    {
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder 100 caracteres")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RolID { get; set; }

        public int? BodegaID { get; set; }

        public bool Activo { get; set; } = true;
    }
}


