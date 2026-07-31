using IDMS.Modules.Api.Master.Dto.Request.MstBrand;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services;

public interface IMstBrandService
{
    Task<(List<ResMstBrandDto> Data, int Total)> GetListAsync(ReqMstBrandDto request);

    Task<ResMstBrandDto?> GetBrandByIdAsync(int id);
    Task<ResMstBrandDto> CreateAsync(ReqMstBrandCreateDto request);
    Task<ResMstBrandDto> UpdateAsync(int id, ReqMstBrandUpdateDto request);
    Task SoftDeleteAsync(int id);
}
