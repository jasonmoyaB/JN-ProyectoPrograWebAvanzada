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
    }
}

