namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiUsuarioService
    {
        Task<List<UsuarioDto>> GetAllAsync(bool? activo = null);
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CrearUsuarioDto dto);
        Task<bool> UpdateAsync(int id, EditarUsuarioDto dto);
        Task<bool> ToggleActivoAsync(int id);
    }

    public class UsuarioDto
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int RolID { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public int? BodegaID { get; set; }
        public string? BodegaNombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class CrearUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int RolID { get; set; }
        public int? BodegaID { get; set; }
    }

    public class EditarUsuarioDto
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public int RolID { get; set; }
        public int? BodegaID { get; set; }
        public bool Activo { get; set; } = true;
    }
}

