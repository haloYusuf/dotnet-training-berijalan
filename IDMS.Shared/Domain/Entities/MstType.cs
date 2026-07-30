using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("mst_types")]
    public class MstType : BaseEntity
    {
        public int BrandId { get; set; }

        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public int Year { get; set; }

        // Relasi ke tabel MstBrand
        public virtual MstBrand Brand { get; set; } = null!;
    }
}