using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("mst_models")]
    public class MstModel : BaseEntity
    {
        public int TypeId { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int Year { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        // Navigation Property ke MstType
        public virtual MstType Type { get; set; } = null!;
    }
}