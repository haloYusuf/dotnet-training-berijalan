using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface IApplicationService
    {
        Task<(List<ApplicationItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit);
        Task<(ApplicationItem? Data, string? Error)> GetByIdAsync(int id);
        Task<(ApplicationItem? Data, string? Error)> CreateAsync(ApplicationRequestDto request);
        Task<(ApplicationItem? Data, string? Error)> UpdateAsync(int id, ApplicationRequestDto request);
        Task<(bool Success, string? Error)> UpdateStatusAsync(int id, string status);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class ApplicationItem
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty; // Asumsi dikembalikan oleh backend
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;    // Asumsi dikembalikan oleh backend
        public decimal OtrPrice { get; set; }
        public decimal DpAmount { get; set; }
        public int TenorMonth { get; set; }
        public double InterestRate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ApplicationRequestDto
    {
        public int CustomerId { get; set; }
        public int ModelId { get; set; }
        public decimal OtrPrice { get; set; }
        public decimal DpAmount { get; set; }
        public int TenorMonth { get; set; }
        public double InterestRate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}