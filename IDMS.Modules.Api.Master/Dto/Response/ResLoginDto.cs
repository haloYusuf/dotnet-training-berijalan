using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResLoginDto
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}