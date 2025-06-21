using System.Text;
using System.Text.Json;
using CathSpeak.Web.Models.DTOs;

namespace CathSpeak.Web.Services
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint, string? token = null);
        Task<T?> PostAsync<T>(string endpoint, object data, string? token = null);
        Task<T?> PutAsync<T>(string endpoint, object data, string? token = null);
        Task<bool> DeleteAsync(string endpoint, string? token = null);
        Task<ApiResponse<T>> GetWithResponseAsync<T>(string endpoint, string? token = null);
        Task<ApiResponse<T>> PostWithResponseAsync<T>(string endpoint, object data, string? token = null);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<ApiService> _logger;

        public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("CathSpeakAPI");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
        {
            try
            {
                SetAuthToken(token);
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, _jsonOptions);
                }

                _logger.LogWarning($"API GET failed: {endpoint} - {response.StatusCode}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API GET error: {endpoint}");
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                SetAuthToken(token);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                }

                _logger.LogWarning($"API POST failed: {endpoint} - {response.StatusCode}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API POST error: {endpoint}");
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                SetAuthToken(token);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
                }

                _logger.LogWarning($"API PUT failed: {endpoint} - {response.StatusCode}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API PUT error: {endpoint}");
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, string? token = null)
        {
            try
            {
                SetAuthToken(token);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API DELETE error: {endpoint}");
                return false;
            }
        }

        public async Task<ApiResponse<T>> GetWithResponseAsync<T>(string endpoint, string? token = null)
        {
            try
            {
                SetAuthToken(token);
                var response = await _httpClient.GetAsync(endpoint);
                var content = await response.Content.ReadAsStringAsync();

                return new ApiResponse<T>
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Data = response.IsSuccessStatusCode ? JsonSerializer.Deserialize<T>(content, _jsonOptions) : default,
                    ErrorMessage = response.IsSuccessStatusCode ? null : content
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API GET error: {endpoint}");
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<T>> PostWithResponseAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                SetAuthToken(token);
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return new ApiResponse<T>
                {
                    IsSuccess = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    Data = response.IsSuccessStatusCode ? JsonSerializer.Deserialize<T>(responseContent, _jsonOptions) : default,
                    ErrorMessage = response.IsSuccessStatusCode ? null : responseContent
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API POST error: {endpoint}");
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private void SetAuthToken(string? token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }
}