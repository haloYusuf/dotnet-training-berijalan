using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResMstInsuranceDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CoverageType { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
    }
}