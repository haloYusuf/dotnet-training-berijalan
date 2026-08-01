using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface IDealerService
    {
        Task<(List<DealerItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit);
        Task<(DealerItem? Data, string? Error)> GetByIdAsync(int id);
        Task<(DealerItem? Data, string? Error)> CreateAsync(DealerRequestDto request);
        Task<(DealerItem? Data, string? Error)> UpdateAsync(int id, DealerRequestDto request);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class DealerItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class DealerRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}