using System.ComponentModel.DataAnnotations;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels
{
    public class RegistroViewModel
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string CorreoElectronico { get; set; }

        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public int RolID { get; set; }
    }
}
