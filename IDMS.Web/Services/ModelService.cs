using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Web.Middleware;

namespace IDMS.Web.Services
{
    public class ModelService : IModelService
    {
        private readonly ApiClient _api;

        private const string _URL = "/api/model";

        public ModelService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(ModelItem? Data, string? Error)> CreateAsync(int typeId, string code, string name, int year, decimal price, int stock)
        {
            try
            {
                var payload = new { typeId, code, name, year, price, stock };
                var res = await _api.PostAsync<ModelItem>(_URL, payload);

                if (res.Status?.ToLower() == "error")
                    return (null, res.Message);

                return (res.Data, null);
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
                var result = await _api.DeleteAsync<object>($"{_URL}/{id}");

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

        public async Task<(ModelItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                var result = await _api.GetAsync<ModelItem>($"{_URL}/{id}");

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

        public async Task<(List<ModelItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit)
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

                var result = await _api.GetAsync<List<ModelItem>>(_URL, query);

                if (result.Status?.ToLower() == "error")
                    return ([], null, result.Message);

                return (result.Data ?? new List<ModelItem>(), result.Pagination, null);
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

        public async Task<(ModelItem? Data, string? Error)> UpdateAsync(int id, int typeId, string code, string name, int year, decimal price, int stock, bool isActive)
        {
            try
            {
                var payload = new { typeId, code, name, year, price, stock, isActive };

                var result = await _api.PutAsync<ModelItem>($"{_URL}/{id}", payload);

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (ApiException ex) { return (null, ex.Message); }
            catch (HttpRequestException ex) { return (null, $"Connection failed: {ex.Message}"); }
        }
    }
}