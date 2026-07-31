using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("mst_insurance")]
    public class MstInsurance : BaseEntity
    {
        [Required]
        public string Code { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string CoverageType { get; set; } = null!;

        [Required]
        public decimal Rate { get; set; }
    }
}