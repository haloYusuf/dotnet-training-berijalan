namespace IDMS.Modules.Api.Master.Dto.Response;

public class ResMstBrandDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
