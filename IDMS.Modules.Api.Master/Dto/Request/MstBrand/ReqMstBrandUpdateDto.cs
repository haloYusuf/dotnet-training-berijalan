using System.ComponentModel.DataAnnotations;

namespace IDMS.Modules.Api.Master.Dto.Request.MstBrand;

public class ReqMstBrandUpdateDto
{
    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;
}
