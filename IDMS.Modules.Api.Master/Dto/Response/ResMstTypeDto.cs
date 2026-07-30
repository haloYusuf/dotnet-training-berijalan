using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResMstTypeDto
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!; // Tambahan agar hasil API lebih informatif
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Year { get; set; }
        public bool IsActive { get; set; }
    }
}