using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.TrnApplication;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Utils;

namespace IDMS.Modules.Api.Master.Services.Impl
{
    public class TrnApplicationService : ITrnApplicationService
    {
        private readonly AppDbContext _context;

        private readonly ICurrentUserServices _user;

        public TrnApplicationService(AppDbContext context, ICurrentUserServices user)
        {
            _context = context;
            _user = user;
        }
        
        public Task<ResTrnApplicationDto> CreateAsync(ReqTrnApplicationCreateDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ResTrnApplicationDto?> GetApplicationByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<(IEnumerable<ResTrnApplicationDto> data, int total)> GetListAsync(ReqTrnApplicationDto request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SoftDeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResTrnApplicationDto> UpdateAsync(int id, ReqTrnApplicationUpdateDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ResTrnApplicationDto> UpdateStatusAsync(int id, string status)
        {
            throw new NotImplementedException();
        }
    }
}