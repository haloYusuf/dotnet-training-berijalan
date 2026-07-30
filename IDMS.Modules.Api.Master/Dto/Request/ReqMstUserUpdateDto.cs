using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request
{
    public class ReqMstUserUpdateDto : ReqMstUserCreateDto
    {
        public bool IsActive { get; set; }
    }
}