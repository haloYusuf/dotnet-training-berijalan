using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDMS.Shared.Domain.Entities
{
    [Table("trn_vehicle_delivery")]
    public class TrnVehicleDelivery : BaseEntity
    {
        [Required]
        public string DeliveryNo { get; set; } = string.Empty;

        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public int DealerId { get; set; }

        [Required]
        public int InsuranceId { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [Required]
        public string DriverName { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? DriverPhone { get; set; }

        [MaxLength(15)]
        public string? PlatNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Notes { get; set; }

        public virtual TrnApplication Application { get; set; } = null!;
        public virtual MstDealer Dealer { get; set; } = null!;
        public virtual MstInsurance Insurance { get; set; } = null!;
    }
}