using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResTrnVehicleDeliveryDto
    {
        public int Id { get; set; }
        public int DealerId { get; set; }
        public int InsuranceId { get; set; }
        public int ApplicationId { get; set; }
        public string DeliveryNo { get; set; } = string.Empty;
        public string DealerName { get; set; } = string.Empty;
        public string InsuranceName { get; set; } = string.Empty;
        public DateTime DeliveryDate { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string? DriverPhone { get; set; }
        public string? PlatNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}