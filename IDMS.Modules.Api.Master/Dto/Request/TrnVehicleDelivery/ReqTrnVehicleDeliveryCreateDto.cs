using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request.TrnVehicleDelivery
{
    public class ReqTrnVehicleDeliveryCreateDto
    {
        [Required(ErrorMessage = "Application is required")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Dealer is required")]
        public int DealerId { get; set; }

        [Required(ErrorMessage = "Insurance is required")]
        public int InsuranceId { get; set; }

        [Required(ErrorMessage = "Delivery Date is required")]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Driver Name is required")]
        public string DriverName { get; set; } = null!;

        [MaxLength(15, ErrorMessage = "Driver Phone length cannot more than 15 Char")]
        public string? DriverPhone { get; set; }

        [MaxLength(15, ErrorMessage = "Plat Number length cannot more than 15 Char")]
        public string? PlatNumber { get; set; }

        [Required(ErrorMessage = "Status is required"), AllowedValues("PLANNED", "IN_TRANSIT", "DELIVERED", "CANCELLED", ErrorMessage = "Status Type is not valid")]
        public string Status { get; set; } = null!;

        [MaxLength(255)]
        public string? Notes { get; set; }
    }
}