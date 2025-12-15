
using System.ComponentModel.DataAnnotations;

public class UsuarioPerfilViewModel
{
    public int UsuarioID { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string CorreoElectronico { get; set; }

    public string Rol { get; set; }

    public bool Activo { get; set; }
}
