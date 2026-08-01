using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface IInsuranceService
    {
        Task<(List<InsuranceItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit);
        Task<(InsuranceItem? Data, string? Error)> GetByIdAsync(int id);
        Task<(InsuranceItem? Data, string? Error)> CreateAsync(InsuranceRequestDto request);
        Task<(InsuranceItem? Data, string? Error)> UpdateAsync(int id, InsuranceRequestDto request);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class InsuranceItem
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CoverageType { get; set; } = string.Empty;
        public double Rate { get; set; }
    }

    public class InsuranceRequestDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CoverageType { get; set; } = string.Empty;
        public double Rate { get; set; }
    }
}