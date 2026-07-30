using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface IModelService
    {
        Task<(List<ModelItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit);

        Task<(ModelItem? Data, string? Error)> GetByIdAsync(int id);

        Task<(ModelItem? Data, string? Error)> CreateAsync(int typeId, string code, string name, int year, decimal price, int stock);

        Task<(ModelItem? Data, string? Error)> UpdateAsync(int id, int typeId, string code, string name, int year, decimal price, int stock, bool isActive);

        Task<(bool Success, string? Error)> DeleteAsync(int id);

    }

    public class ModelItem
    {
        public int Id { get; set; }
        public int typeId { get; set; }
        public string typeName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
    }
}