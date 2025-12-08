using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiAuthService : IApiAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiAuthService> _logger;

        public ApiAuthService(IHttpClientFactory httpClientFactory, ILogger<ApiAuthService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    return result;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return null;
                }
                else
                {
                    _logger.LogError("Error en login: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al API de login");
                return null;
            }
        }

        public async Task<bool> CambiarContrasenaAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var request = new
                {
                    UsuarioID = userId,
                    ContrasenaActual = currentPassword,
                    NuevaContrasena = newPassword
                };

                var response = await _httpClient.PostAsJsonAsync("api/auth/cambiar-contrasena", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar contraseña");
                return false;
            }
        }

        public async Task<string> RecuperarClaveAsync(string email)
        {
            try
            {
                var request = new { Email = email };
                var response = await _httpClient.PostAsJsonAsync("api/auth/recuperar-clave", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (result.TryGetProperty("claveTemporal", out var clave))
                    {
                        return clave.GetString() ?? string.Empty;
                    }
                }
                
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al recuperar clave");
                return string.Empty;
            }
        }
    }
}

