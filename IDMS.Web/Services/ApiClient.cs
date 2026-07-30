using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IDMS.Web.Middleware;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace IDMS.Web.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiClient(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            SetHeaders();
            using var response = await _http.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            SetHeaders();
            var qs = string.Join("&", queryParams.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
            using var response = await _http.GetAsync($"{endpoint}?{qs}");
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            SetHeaders();
            var json = JsonSerializer.Serialize(data, JsonOptions);
            using var response = await _http.PostAsync(endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            SetHeaders();
            var json = JsonSerializer.Serialize(data, JsonOptions);
            using var response = await _http.PutAsync(endpoint, new StringContent(json, Encoding.UTF8, "application/json"));
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            SetHeaders();
            using var response = await _http.DeleteAsync(endpoint);
            return await HandleResponse<T>(response);
        }

        private void SetHeaders()
        {
            _http.DefaultRequestHeaders.Remove("X-Api-Key");
            _http.DefaultRequestHeaders.Add("X-Api-Key", "rahasia");

            var token = _httpContextAccessor.HttpContext?.User.FindFirst("JwtToken")?.Value;
            _http.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token)
                ? new AuthenticationHeaderValue("Bearer", token)
                : null;
        }

        private static async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errMsg = TryExtractMessage(body);
                throw new ApiException((int)response.StatusCode, errMsg ?? $"Server returned {(int)response.StatusCode}");
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return new ApiResponse<T> { Status = "Error", Message = "Server returned empty response" };
            }

            try
            {
                return JsonSerializer.Deserialize<ApiResponse<T>>(body, JsonOptions)
                    ?? new ApiResponse<T> { Status = "Error", Message = "Failed to parse response" };
            }
            catch (JsonException ex)
            {
                throw new ApiException(500, $"Invalid response format: {ex.Message}");
            }
        }

        private static string? TryExtractMessage(string body)
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                    return msg.GetString();

                if (root.TryGetProperty("Message", out var msg2) && msg2.ValueKind == JsonValueKind.String)
                    return msg2.GetString();

                if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
                    return title.GetString();
            }
            catch { }

            return null;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public class ApiResponse<T>
    {
        public string ReqId { get; set; } = string.Empty;
        public string Status { get; set; } = "Success";
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Pagination? Pagination { get; set; }
    }

    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int Limit { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}