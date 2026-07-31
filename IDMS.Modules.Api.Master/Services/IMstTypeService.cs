using IDMS.Modules.Api.Master.Dto.Request.MstType;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services;

public interface IMstTypeService
{
    Task<(IEnumerable<ResMstTypeDto> data, int total)> GetListAsync(ReqMstTypeDto request);
    Task<ResMstTypeDto?> GetTypeByIdAsync(int id);
    Task<ResMstTypeDto> CreateAsync(ReqMstTypeCreateDto request);
    Task<ResMstTypeDto> UpdateAsync(int id, ReqMstTypeUpdateDto request);
    Task<bool> SoftDeleteAsync(int id);
}