


using System.Net.Http.Json;
using System.Text.Json;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Services
{
    public class ApiBodegaService : IApiBodegaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiBodegaService> _logger;

        public ApiBodegaService(IHttpClientFactory httpClientFactory, ILogger<ApiBodegaService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _logger = logger;
        }

        public async Task<List<BodegaDto>> GetAllAsync(bool? activo = null)
        {
            try
            {
                var url = activo.HasValue ? $"api/bodegas?activo={activo.Value}" : "api/bodegas";
                var response = await _httpClient.GetFromJsonAsync<List<BodegaDto>>(url);
                return response ?? new List<BodegaDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodegas");
                return new List<BodegaDto>();
            }
        }

        public async Task<BodegaDto?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<BodegaDto>($"api/bodegas/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bodega {Id}", id);
                return null;
            }
        }

        public async Task<int> CreateAsync(CrearBodegaDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/bodegas", dto);
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
                _logger.LogError(ex, "Error al crear bodega");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, CrearBodegaDto dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/bodegas/{id}", dto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar bodega {Id}", id);
                return false;
            }
        }

        public async Task<bool> ToggleActivoAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/bodegas/{id}/toggle-activo", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de bodega {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/bodegas/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar bodega {Id}", id);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var bodega = await GetByIdAsync(id);
                return bodega != null;
            }
            catch
            {
                return false;
            }
        }
    }
}

