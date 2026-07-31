using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request.MstType
{
    public class ReqMstTypeCreateDto
    {
        [Required]
        public int BrandId { get; set; }

        [Required, MaxLength(10)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, Range(1900, 9999)]
        public int Year { get; set; }
    }
}