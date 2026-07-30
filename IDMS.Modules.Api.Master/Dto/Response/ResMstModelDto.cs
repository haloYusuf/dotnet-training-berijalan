using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResMstModelDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!; // Menampilkan nama tipe agar hasil API informatif
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
    }
}