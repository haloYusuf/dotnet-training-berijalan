using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Modules.Api.Master.Dto.Request.TrnApplication;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface ITrnApplicationService
    {
        Task<(IEnumerable<ResTrnApplicationDto> data, int total)> GetListAsync(ReqTrnApplicationDto request);

        Task<ResTrnApplicationDto?> GetApplicationByIdAsync(int id);

        Task<ResTrnApplicationDto> CreateAsync(ReqTrnApplicationCreateDto request);

        Task<ResTrnApplicationDto> UpdateAsync(int id, ReqTrnApplicationUpdateDto request);
        

        Task<ResTrnApplicationDto> UpdateStatusAsync(int id, string status);
        Task<bool> SoftDeleteAsync(int id);
    }
}