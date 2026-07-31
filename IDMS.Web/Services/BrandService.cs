using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Web.Middleware;

namespace IDMS.Web.Services
{
    public class BrandService : IBrandService
    {
        private readonly ApiClient _api;

        public BrandService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(List<BrandItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? search, int page, int limit)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                    ["Page"] = page.ToString(),
                    ["Limit"] = limit.ToString()
                };
                if (!string.IsNullOrEmpty(search))
                    query["Keyword"] = search;

                var result = await _api.GetAsync<List<BrandItem>>("/api/brand", query);

                if (result.Status == "Error")
                    return ([], null, result.Message);

                return (result.Data ?? [], result.Pagination, null);
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

        public async Task<(BrandItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                var result = await _api.GetAsync<BrandItem>($"/api/brand/{id}");

                if (result.Status == "Error")
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

        public async Task<(BrandItem? Data, string? Error)> CreateAsync(string code, string name)
        {
            try
            {
                var result = await _api.PostAsync<BrandItem>("/api/brand", new { code, name });

                if (result.Status == "Error")
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

        public async Task<(BrandItem? Data, string? Error)> UpdateAsync(int id, string code, string name)
        {
            try
            {
                var result = await _api.PutAsync<BrandItem>($"/api/brand/{id}", new { code, name });

                if (result.Status == "Error")
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
                var result = await _api.DeleteAsync<object>($"/api/brand/{id}");

                if (result.Status == "Error")
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
    }
}