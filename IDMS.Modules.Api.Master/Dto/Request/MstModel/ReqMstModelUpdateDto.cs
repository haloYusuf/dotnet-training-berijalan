using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request.MstModel
{
    public class ReqMstModelUpdateDto : ReqMstModelCreateDto
    {
        public bool IsActive { get; set; }
    }
}