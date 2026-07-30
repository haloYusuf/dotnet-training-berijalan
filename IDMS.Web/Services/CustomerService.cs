using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Web.Middleware;

namespace IDMS.Web.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApiClient _api;

        private const string _URL = "/api/customer";

        public CustomerService(ApiClient api)
        {
            _api = api;
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            try
            {
                var result = await _api.DeleteAsync<object>($"{_URL}/{id}");

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

        public async Task<(CustomerItem? Data, string? Error)> GetByIdAsync(int id)
        {
            try
            {
                var result = await _api.GetAsync<CustomerItem>($"{_URL}/{id}");

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

        public async Task<(List<CustomerItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? search, int page, int limit)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                    ["Page"] = page.ToString(),
                    ["Limit"] = limit.ToString()
                };
                if (!string.IsNullOrEmpty(search))
                    query["Search"] = search;

                var result = await _api.GetAsync<List<CustomerItem>>(_URL, query);

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

        public async Task<(CustomerItem? Data, string? Error)> CreateAsync(string nik, string FullName, DateTime? birthDate, string phone, string email, string address)
        {
            try
            {
                var result = await _api.PostAsync<CustomerItem>(_URL, new { nik, FullName, birthDate, phone, email, address });

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

        public async Task<(CustomerItem? Data, string? Error)> UpdateAsync(int id, string nik, string FullName, DateTime? birthDate, string phone, string email, string address)
        {
            try
            {
                var result = await _api.PutAsync<CustomerItem>($"{_URL}/{id}", new { nik, FullName, birthDate, phone, email, address });

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
    }
}