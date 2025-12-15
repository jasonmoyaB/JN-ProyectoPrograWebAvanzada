using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiSolicitudService : IApiSolicitudService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiSolicitudService> _logger;

        public ApiSolicitudService(IHttpClientFactory httpClientFactory, ILogger<ApiSolicitudService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<SolicitudDto>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SolicitudDto>>("api/solicitudes");
                return response ?? new List<SolicitudDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las solicitudes");
                return new List<SolicitudDto>();
            }
        }

        public async Task<List<SolicitudDto>> GetByUsuarioAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SolicitudDto>>($"api/solicitudes/usuario/{usuarioId}");
                return response ?? new List<SolicitudDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes por usuario {UsuarioId}", usuarioId);
                return new List<SolicitudDto>();
            }
        }

        public async Task<List<SolicitudDto>> GetByBodegaAsync(int bodegaId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SolicitudDto>>($"api/solicitudes/bodega/{bodegaId}");
                return response ?? new List<SolicitudDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitudes por bodega {BodegaId}", bodegaId);
                return new List<SolicitudDto>();
            }
        }

        public async Task<SolicitudDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<SolicitudDto>($"api/solicitudes/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener solicitud {Id}", id);
                return null;
            }
        }

        public async Task<int> CreateAsync(CrearSolicitudDto dto, int usuarioId)
        {
            try
            {
                var request = new
                {
                    Dto = dto,
                    UsuarioID = usuarioId
                };

                var response = await _httpClient.PostAsJsonAsync("api/solicitudes", request);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (result.TryGetProperty("id", out var id))
                    {
                        return id.GetInt32();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear solicitud");
                throw;
            }
        }

        public async Task<int> GetCountPendientesByUsuarioAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"api/solicitudes/usuario/{usuarioId}/count-pendientes");
                if (response.TryGetProperty("count", out var count))
                {
                    return count.GetInt32();
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener conteo de solicitudes pendientes por usuario {UsuarioId}", usuarioId);
                return 0;
            }
        }

        public async Task AprobarAsync(int id, int usuarioId, string? comentarios = null)
        {
            try
            {
                var request = new { comentarios, usuarioID = usuarioId };
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/aprobar", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al aprobar solicitud: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al aprobar solicitud {Id}", id);
                throw;
            }
        }

        public async Task RechazarAsync(int id, int usuarioId, string? comentarios = null)
        {
            try
            {
                var request = new { comentarios, usuarioID = usuarioId };
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/rechazar", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al rechazar solicitud: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al rechazar solicitud {Id}", id);
                throw;
            }
        }

        public async Task EnviarAsync(int id, string? comentarios = null)
        {
            try
            {
                var request = new { comentarios };
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/enviar", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al enviar solicitud: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar solicitud {Id}", id);
                throw;
            }
        }

        public async Task EntregarAsync(int id, int bodegaOrigenId, int bodegaDestinoId, int usuarioId, string? comentarios = null)
        {
            try
            {
                var request = new { bodegaOrigenID = bodegaOrigenId, bodegaDestinoID = bodegaDestinoId, usuarioID = usuarioId, comentarios };
                var response = await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/entregar", request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al entregar solicitud: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al entregar solicitud {Id}", id);
                throw;
            }
        }

        public async Task EstablecerCantidadesEnviadasAsync(int id, EstablecerCantidadesEnviadasDto dto)
        {
            try
            {
                await _httpClient.PutAsJsonAsync($"api/solicitudes/{id}/establecer-cantidades", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al establecer cantidades enviadas para solicitud {Id}", id);
                throw;
            }
        }

        public async Task<bool> TieneCantidadesEnviadasAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"api/solicitudes/{id}/tiene-cantidades-enviadas");
                if (response.TryGetProperty("tieneCantidades", out var tiene))
                {
                    return tiene.GetBoolean();
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si tiene cantidades enviadas para solicitud {Id}", id);
                return false;
            }
        }
    }
}

