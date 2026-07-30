using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Modules.Api.Master.Dto.Request;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface IMstCustomerService
    {
        Task<(IEnumerable<ResMstCustomerDto> data, int total)> GetListAsync(ReqMstCustomerDto request);

        Task<ResMstCustomerDto?> GetCustomerByIdAsync(int id);
        Task<ResMstCustomerDto> CreateAsync(ReqMstCustomerCreateDto request);
        Task<ResMstCustomerDto> UpdateAsync(int id, ReqMstCustomerUpdateDto request);
        Task<bool> SoftDeleteAsync(int id);
    }
}