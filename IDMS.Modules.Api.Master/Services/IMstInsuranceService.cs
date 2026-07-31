using IDMS.Modules.Api.Master.Dto.Request.MstInsurance;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface IMstInsuranceService
    {
        Task<(IEnumerable<ResMstInsuranceDto> data, int total)> GetListAsync(ReqMstInsuranceDto request);

        Task<ResMstInsuranceDto?> GetInsuranceByIdAsync(int id);
        Task<ResMstInsuranceDto> CreateAsync(ReqMstInsuranceCreateDto request);
        Task<ResMstInsuranceDto> UpdateAsync(int id, ReqMstInsuranceUpdateDto request);
        Task<bool> SoftDeleteAsync(int id);
    }
}