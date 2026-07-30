using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Modules.Api.Master.Dto.Request;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface IAuthService
    {
        Task<bool> registerAsync(ReqMstUserCreateDto req);

        Task<ResLoginDto> loginAsync(ReqLoginDto req);
    }
}