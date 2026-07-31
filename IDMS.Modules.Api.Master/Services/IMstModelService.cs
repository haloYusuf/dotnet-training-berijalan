using IDMS.Modules.Api.Master.Dto.Request.MstModel;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface IMstModelService
    {
        Task<(IEnumerable<ResMstModelDto> data, int total)> GetListAsync(ReqMstModelDto request);
        Task<ResMstModelDto?> GetTypeByIdAsync(int id);
        Task<ResMstModelDto> CreateAsync(ReqMstModelCreateDto request);
        Task<ResMstModelDto> UpdateAsync(int id, ReqMstModelUpdateDto request);
        Task<bool> SoftDeleteAsync(int id);
    }
}