using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public class DealerService : IDealerService
    {
        private readonly ApiClient _api;
        private const string _URL = "/api/dealer";

        public DealerService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(List<DealerItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit)
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

                var result = await _api.GetAsync<List<DealerItem>>(_URL, query);

                if (result.Status?.ToLower() == "error")
                    return ([], null, result.Message);

                return (result.Data ?? new List<DealerItem>(), result.Pagination, null);
            }
            catch (Exception ex)
            {
                return ([], null, ex.Message);
            }
        }

        public async Task<(DealerItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                var result = await _api.GetAsync<DealerItem>($"{_URL}/{id}");

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(DealerItem? Data, string? Error)> CreateAsync(DealerRequestDto request)
        {
            try
            {
                var result = await _api.PostAsync<DealerItem>(_URL, request);

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(DealerItem? Data, string? Error)> UpdateAsync(int id, DealerRequestDto request)
        {
            try
            {
                var result = await _api.PutAsync<DealerItem>($"{_URL}/{id}", request);

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
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
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}