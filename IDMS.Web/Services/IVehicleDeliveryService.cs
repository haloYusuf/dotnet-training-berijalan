using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Web.Services
{
    public interface IVehicleDeliveryService
    {
        Task<(List<VehicleDeliveryItem> Data, Pagination? Pagination, string? Error)> GetListAsync(string? keyword, int page, int limit);
        Task<(VehicleDeliveryItem? Data, string? Error)> GetByIdAsync(int id);
        Task<(VehicleDeliveryItem? Data, string? Error)> CreateAsync(VehicleDeliveryRequestDto request);
        Task<(VehicleDeliveryItem? Data, string? Error)> UpdateAsync(int id, VehicleDeliveryRequestDto request);
        Task<(bool Success, string? Error)> UpdateStatusAsync(int id, string status);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }

    public class VehicleDeliveryItem
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int DealerId { get; set; }
        public string DealerName { get; set; } = string.Empty;       // Asumsi direlasikan backend
        public int InsuranceId { get; set; }
        public string InsuranceName { get; set; } = string.Empty;    // Asumsi direlasikan backend
        public DateTime DeliveryDate { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string? DriverPhone { get; set; }
        public string? PlatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class VehicleDeliveryRequestDto
    {
        public int ApplicationId { get; set; }
        public int DealerId { get; set; }
        public int InsuranceId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string? DriverPhone { get; set; }
        public string? PlatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}