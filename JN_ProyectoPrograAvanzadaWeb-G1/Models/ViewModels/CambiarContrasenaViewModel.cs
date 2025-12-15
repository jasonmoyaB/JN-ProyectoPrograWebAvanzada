

using System.ComponentModel.DataAnnotations;

public class CambiarContrasenaViewModel
{
    public int UsuarioID { get; set; }

    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    public string ContrasenaActual { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "Debe tener al menos 6 caracteres.")]
    public string NuevaContrasena { get; set; }

    [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarContrasena { get; set; }
}
