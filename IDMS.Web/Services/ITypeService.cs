using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface ITypeService
    {
        Task<(List<TypeItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit);

        Task<(TypeItem? Data, string? Error)> GetByIdAsync(int id);

        Task<(TypeItem? Data, string? Error)> CreateAsync(int brandId, string code, string name, int year);

        Task<(TypeItem? Data, string? Error)> UpdateAsync(int id, int brandId, string code, string name, int year, bool isActive);

        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class TypeItem
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public bool IsActive { get; set; }
    }
}