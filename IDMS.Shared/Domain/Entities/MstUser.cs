using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Domain.Entities
{
    [Table("mst_users")]
    public class MstUser : BaseEntity
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public string Role { get; set; } = "customer";
    }
}