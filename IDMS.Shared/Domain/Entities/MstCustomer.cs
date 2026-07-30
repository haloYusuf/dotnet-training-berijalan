using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("mst_customer")]
    public class MstCustomer : BaseEntity
    {
        public string Nik { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}