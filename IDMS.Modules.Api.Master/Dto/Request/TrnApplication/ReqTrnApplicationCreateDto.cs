using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request.TrnApplication
{
    public class ReqTrnApplicationCreateDto
    {
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Model is required")]
        public int ModelId { get; set; }

        [Required(ErrorMessage = "Otr Price is required")]
        public decimal OtrPrice { get; set; }


        [Required(ErrorMessage = "DP Amount is required")]
        public decimal DpAmount { get; set; }


        [Required(ErrorMessage = "Tenor Month is required")]
        public int TenorMonth { get; set; }

        public decimal InterestRate { get; set; } = 6.0m;

        [AllowedValues("DRAFT", "SUBMITTED", "APPROVED", "REJECTED", ErrorMessage = "Status Type is not valid")]
        public string Status { get; set; } = "DRAFT";
    }
}