namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public interface IApiRolService
    {
        Task<List<RolDto>> GetAllAsync();
    }

    public class RolDto
    {
        public int RolID { get; set; }
        public string NombreRol { get; set; } = string.Empty;
    }
}

