using JN_ProyectoPrograAvanzadaWeb_G1.Models;

public class Usuario
{
    public int UsuarioID { get; set; }
    public string Nombre { get; set; }
    public string CorreoElectronico { get; set; }
    public string ContrasenaHash { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    public int RolID { get; set; } 
    public Rol Rol { get; set; }   
}
