using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Shared.Common
{
    public class JwtUserDto
    {
        public string? Id { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }
    }
}