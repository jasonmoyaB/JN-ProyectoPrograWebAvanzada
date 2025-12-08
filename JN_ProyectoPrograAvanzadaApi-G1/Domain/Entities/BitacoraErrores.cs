namespace JN_ProyectoPrograAvanzadaApi_G1.Domain.Entities
{
    public class BitacoraErrores
    {
        public int BitacoraErrorID { get; set; }
        public int? UsuarioID { get; set; }
        public string Error { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? Controlador { get; set; }
        public string? Accion { get; set; }
        public DateTime FechaUTC { get; set; } = DateTime.UtcNow;
        public string? RequestPath { get; set; }
        public string? UserAgent { get; set; }
    }
}

