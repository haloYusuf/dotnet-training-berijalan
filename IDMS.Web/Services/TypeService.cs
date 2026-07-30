using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Web.Middleware;

namespace IDMS.Web.Services
{
    public class TypeService : ITypeService
    {
        private readonly ApiClient _api;

        public TypeService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(TypeItem? Data, string? Error)> CreateAsync(int brandId, string code, string name, int year)
        {
            try
            {
                // Payload akan otomatis di-serialize menjadi JSON { "brandId": 1, "code": "...", "name": "...", "year": 2024 }
                var payload = new { brandId, code, name, year };

                // ApiClient sudah otomatis menyematkan Bearer Token dan X-Api-Key!
                var result = await _api.PostAsync<TypeItem>("/api/type", payload);

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (ApiException ex)
            {
                return (null, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Connection failed: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            try
            {
                // Memanggil endpoint sesungguhnya: DELETE /api/type/:id
                var result = await _api.DeleteAsync<object>($"/api/type/{id}");

                if (result.Status?.ToLower() == "error")
                    return (false, result.Message);

                return (true, null);
            }
            catch (ApiException ex)
            {
                return (false, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Connection failed: {ex.Message}");
            }
        }

        public async Task<(TypeItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                // Memanggil endpoint GET /api/type/{id} menggunakan ApiClient
                var result = await _api.GetAsync<TypeItem>($"/api/type/{id}");

                // Cek jika status dari JSON Backend adalah "error" (dibuat toLower agar aman)
                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                // Jika sukses, kembalikan datanya dan error-nya null
                return (result.Data, null);
            }
            catch (ApiException ex)
            {
                // Menangkap error format dari ApiClient (misal: 400 Bad Request, 404 Not Found, dsb)
                return (null, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                // Menangkap error jaringan (misal: server Backend mati atau tidak bisa diakses)
                return (null, $"Connection failed: {ex.Message}");
            }
        }

        public async Task<(List<TypeItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                    ["Page"] = page.ToString(),
                    ["Limit"] = limit.ToString()
                };

                if (!string.IsNullOrEmpty(keyword))
                    query["Keyword"] = keyword;

                // Memanggil endpoint /api/type/
                var result = await _api.GetAsync<List<TypeItem>>("/api/type", query);

                if (result.Status?.ToLower() == "error")
                    return ([], null, result.Message);

                return (result.Data ?? new List<TypeItem>(), result.Pagination, null);
            }
            catch (ApiException ex)
            {
                return ([], null, ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return ([], null, $"Connection failed: {ex.Message}");
            }
        }

        public async Task<(TypeItem? Data, string? Error)> UpdateAsync(int id, int brandId, string code, string name, int year, bool isActive)
        {
            try
            {
                // Payload akan otomatis di-serialize menjadi JSON sesuai requirement kamu
                var payload = new { brandId, code, name, year, isActive };

                // Memanggil endpoint PUT /api/type/{id}
                var result = await _api.PutAsync<TypeItem>($"/api/type/{id}", payload);

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (ApiException ex) { return (null, ex.Message); }
            catch (HttpRequestException ex) { return (null, $"Connection failed: {ex.Message}"); }
        }
    }
}