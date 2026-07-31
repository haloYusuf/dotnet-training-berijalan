using IDMS.Modules.Api.Master.Dto.Request.MstDealer;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface IMstDealerService
    {
        Task<(IEnumerable<ResMstDealerDto> data, int total)> GetListAsync(ReqMstDealerDto request);
        Task<ResMstDealerDto?> GetDealerByIdAsync(int id);
        Task<ResMstDealerDto> CreateAsync(ReqMstDealerCreateDto request);
        Task<ResMstDealerDto> UpdateAsync(int id, ReqMstDealerUpdateDto request);
        Task<bool> SoftDeleteAsync(int id);
    }
}