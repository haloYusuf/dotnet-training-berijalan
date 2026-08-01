using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResTrnApplicationDto
    {
        public int id { get; set; }
        public string ApplicationNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public decimal OtrPrice { get; set; }
        public decimal DpAmount { get; set; }
        public int TenorMonth { get; set; }
        public decimal InterestRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}