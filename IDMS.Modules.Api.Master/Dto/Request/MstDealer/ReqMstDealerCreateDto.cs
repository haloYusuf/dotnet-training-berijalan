using System.ComponentModel.DataAnnotations;

namespace IDMS.Modules.Api.Master.Dto.Request.MstDealer
{
    public class ReqMstDealerCreateDto
    {
        [Required(ErrorMessage = "Code is required")]
        [MaxLength(10, ErrorMessage = "Kode maks 10 Character")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name maks 100 Character")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50, ErrorMessage = "City maks 50 Character")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required")]
        [AllowedValues("JABODETABEK", "JABAR", "JATENG", "JATIM", "SUMUT", "SUMSEL", "KALIMANTAN", "SULAWESI", ErrorMessage = "Region isnt valid")]
        public string Region { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Name maks 255 Character")]
        public string? Address { get; set; }

        [MaxLength(15, ErrorMessage = "Name maks 15 Character")]
        public string? Phone { get; set; }

        [MaxLength(50, ErrorMessage = "Name maks 50 Character")]
        public string? Email { get; set; }
    }
}