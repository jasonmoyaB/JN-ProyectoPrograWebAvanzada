namespace JN_ProyectoPrograAvanzadaWeb_G1.Domain.Entities
{
    public class Auditoria
    {
        public int AuditoriaID { get; set; }
        public int UsuarioID { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string? Tabla { get; set; }
        public int? RegistroID { get; set; }
        public string? DatosAntes { get; set; } 
        public string? DatosDespues { get; set; } 
        public DateTime FechaUTC { get; set; } = DateTime.UtcNow;
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }

      
        public Usuario? Usuario { get; set; }
    }
}

