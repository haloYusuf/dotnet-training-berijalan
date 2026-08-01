using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public class InsuranceService : IInsuranceService
    {
        private readonly ApiClient _api;
        private const string _URL = "/api/insurance";

        public InsuranceService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(List<InsuranceItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit)
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

                var result = await _api.GetAsync<List<InsuranceItem>>(_URL, query);

                if (result.Status?.ToLower() == "error")
                    return ([], null, result.Message);

                return (result.Data ?? new List<InsuranceItem>(), result.Pagination, null);
            }
            catch (Exception ex)
            {
                return ([], null, ex.Message);
            }
        }

        public async Task<(InsuranceItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                var result = await _api.GetAsync<InsuranceItem>($"{_URL}/{id}");

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(InsuranceItem? Data, string? Error)> CreateAsync(InsuranceRequestDto request)
        {
            try
            {
                var result = await _api.PostAsync<InsuranceItem>(_URL, request);

                if (result.Status?.ToLower() == "error")
                    return (null, result.Message);

                return (result.Data, null);
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public async Task<(InsuranceItem? Data, string? Error)> UpdateAsync(int id, InsuranceRequestDto request)
        {
            try
            {
                var result = await _api.PutAsync<InsuranceItem>($"{_URL}/{id}", request);

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