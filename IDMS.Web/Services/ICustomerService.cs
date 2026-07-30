using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface ICustomerService
    {
        Task<(List<CustomerItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? search, int page, int limit);
        Task<(CustomerItem? Data, string? Error)> GetByIdAsync(int id);
        Task<(CustomerItem? Data, string? Error)> CreateAsync(string nik, string FullName, DateTime? birthDate, string phone, string email, string address);
        Task<(CustomerItem? Data, string? Error)> UpdateAsync(int id, string nik, string FullName, DateTime? birthDate, string phone, string email, string address);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class CustomerItem
    {
        public int Id { get; set; }
        public string Nik { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
    }
}