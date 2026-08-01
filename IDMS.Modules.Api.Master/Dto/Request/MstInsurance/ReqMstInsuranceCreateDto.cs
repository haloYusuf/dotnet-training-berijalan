using System.ComponentModel.DataAnnotations;

namespace IDMS.Modules.Api.Master.Dto.Request.MstInsurance
{
    public class ReqMstInsuranceCreateDto
    {
        [Required(ErrorMessage = "Code is required")]
        [MaxLength(10, ErrorMessage = "Kode maks 10 Character")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Kode maks 100 Character")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Coverage Type is required")]
        [AllowedValues("TLO", "COMPREHENSIVE", "COMBINATION", ErrorMessage = "Coverage Type is not valid")]
        public string CoverageType { get; set; } = null!;

        public decimal Rate { get; set; }
    }
}