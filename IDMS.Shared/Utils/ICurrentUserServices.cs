using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Shared.Common;

namespace IDMS.Shared.Utils
{
    public interface ICurrentUserServices
    {
        JwtUserDto? GetCurrentUser();

        string? GetUserId();

        string? GetEmail();

        string? GetFullName();
    }
}