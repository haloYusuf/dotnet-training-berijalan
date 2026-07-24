using System.ComponentModel.DataAnnotations;

namespace IDMS.Modules.Api.Master.Dto.Request;

public class ReqMstBrandCreateDto
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;
}
