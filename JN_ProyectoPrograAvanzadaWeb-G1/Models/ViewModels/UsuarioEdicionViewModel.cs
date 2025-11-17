using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class UsuarioEdicionViewModel
{
    public int UsuarioID { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string CorreoElectronico { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un rol válido.")]
    public int RolID { get; set; }

    [BindNever]
    public List<SelectListItem> RolesDisponibles { get; set; }

    public bool Activo { get; set; }
}