using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly ApiClient _api;
        private const string _URL = "/api/application";

        public ApplicationService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(List<ApplicationItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit)
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

                var result = await _api.GetAsync<List<ApplicationItem>>(_URL, query);

                if (result.Status?.ToLower() == "error")
                    return ([], null, result.Message);

                return (result.Data ?? new List<ApplicationItem>(), result.Pagination, null);
            }
            catch (Exception ex) { return ([], null, ex.Message); }
        }

        public async Task<(ApplicationItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                var result = await _api.GetAsync<ApplicationItem>($"{_URL}/{id}");
                if (result.Status?.ToLower() == "error") return (null, result.Message);
                return (result.Data, null);
            }
            catch (Exception ex) { return (null, ex.Message); }
        }

        public async Task<(ApplicationItem? Data, string? Error)> CreateAsync(ApplicationRequestDto request)
        {
            try
            {
                var result = await _api.PostAsync<ApplicationItem>(_URL, request);
                if (result.Status?.ToLower() == "error") return (null, result.Message);
                return (result.Data, null);
            }
            catch (Exception ex) { return (null, ex.Message); }
        }

        public async Task<(ApplicationItem? Data, string? Error)> UpdateAsync(int id, ApplicationRequestDto request)
        {
            try
            {
                var result = await _api.PutAsync<ApplicationItem>($"{_URL}/{id}", request);
                if (result.Status?.ToLower() == "error") return (null, result.Message);
                return (result.Data, null);
            }
            catch (Exception ex) { return (null, ex.Message); }
        }

        public async Task<(bool Success, string? Error)> UpdateStatusAsync(int id, string status)
        {
            try
            {
                // Endpoint khusus PUT /api/application/status/{id}
                var result = await _api.PutAsync<object>($"{_URL}/status/{id}", status);
                if (result.Status?.ToLower() == "error") return (false, result.Message);
                return (true, null);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            try
            {
                var result = await _api.DeleteAsync<object>($"{_URL}/{id}");
                if (result.Status?.ToLower() == "error") return (false, result.Message);
                return (true, null);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }
    }
}