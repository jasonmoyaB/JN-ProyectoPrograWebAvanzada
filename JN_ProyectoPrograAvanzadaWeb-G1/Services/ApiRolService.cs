

using System.Net.Http.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiRolService : IApiRolService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiRolService> _logger;

        public ApiRolService(IHttpClientFactory httpClientFactory, ILogger<ApiRolService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<RolDto>> GetAllAsync()
        {
            try
            {
               
                var usuarios = await _httpClient.GetFromJsonAsync<List<UsuarioDto>>("api/usuarios");
                if (usuarios == null) return new List<RolDto>();

                var roles = usuarios
                    .GroupBy(u => new { u.RolID, u.RolNombre })
                    .Select(g => new RolDto { RolID = g.Key.RolID, NombreRol = g.Key.RolNombre })
                    .Distinct()
                    .OrderBy(r => r.RolID)
                    .ToList();

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles");
                
                return new List<RolDto>
                {
                    new RolDto { RolID = 1, NombreRol = "Administrador" },
                    new RolDto { RolID = 2, NombreRol = "Técnico" }
                };
            }
        }
    }
}

