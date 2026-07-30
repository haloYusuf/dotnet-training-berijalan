using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface IBrandService
    {
        Task<(List<BrandItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? search, int page, int limit);
        Task<(BrandItem? Data, string? Error)> GetByIdAsync(int id);
        Task<(BrandItem? Data, string? Error)> CreateAsync(string code, string name);
        Task<(BrandItem? Data, string? Error)> UpdateAsync(int id, string code, string name);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class BrandItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}