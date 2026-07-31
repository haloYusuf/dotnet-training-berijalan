using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IDMS.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace IDMS.Shared.Utils
{
    public class CurrentUserServices : ICurrentUserServices
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public JwtUserDto? GetCurrentUser()
        {
            var user = User;
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                return null;
            }

            return new JwtUserDto
            {
                Id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = user.FindFirst(ClaimTypes.Email)?.Value,
                FullName = user.FindFirst("FullName")?.Value
            };
        }

        public string? GetEmail() => GetCurrentUser()?.Email;

        public string? GetFullName() => GetCurrentUser()?.FullName;

        public string? GetUserId() => GetCurrentUser()?.Id;
    }
}