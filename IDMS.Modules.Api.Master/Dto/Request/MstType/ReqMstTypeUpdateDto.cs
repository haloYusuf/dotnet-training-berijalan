using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request.MstType
{
    public class ReqMstTypeUpdateDto : ReqMstTypeCreateDto
    {
        public bool IsActive { get; set; }
    }
}