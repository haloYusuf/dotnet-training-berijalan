using System.ComponentModel.DataAnnotations;

namespace IDMS.Modules.Api.Master.Dto.Request;

public class ReqMstBrandCreateDto
{
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(3, ErrorMessage = "Kode maks 3 Character")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
}
