using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("mst_dealer")]
    public class MstDealer : BaseEntity
    {
        [Required]
        public string Code { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Address { get; set; } = null!;

        [Required]
        public string City { get; set; } = null!;

        [Required]
        public string Region { get; set; } = null!;

        [MaxLength(15)]
        public string? Phone { get; set; } = null!;

        [MaxLength(50)]
        public string? Email { get; set; } = null!;
    }
}