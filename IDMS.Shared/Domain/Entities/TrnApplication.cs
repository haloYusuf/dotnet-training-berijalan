using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("trn_application")]
    public class TrnApplication : BaseEntity
    {
        [Required]
        public string ApplicationNo { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ModelId { get; set; }

        [Required]
        public decimal OtrPrice { get; set; }

        [Required]
        public decimal DpAmount { get; set; }

        [Required]
        public int TenorMonth { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public virtual MstCustomer Customer { get; set; } = null!;
        public virtual MstModel Model { get; set; } = null!;
    }
}