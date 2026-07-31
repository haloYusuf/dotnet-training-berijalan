using IDMS.Modules.Api.Master.Dto.Request.Auth;
using IDMS.Modules.Api.Master.Dto.Response;

namespace IDMS.Modules.Api.Master.Services
{
    public interface IAuthService
    {
        Task<bool> registerAsync(ReqMstUserCreateDto req);

        Task<ResLoginDto> loginAsync(ReqLoginDto req);
    }
}