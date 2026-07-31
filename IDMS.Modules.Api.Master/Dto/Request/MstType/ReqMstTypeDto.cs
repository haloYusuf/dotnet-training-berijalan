using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request.MstType
{
    public class ReqMstTypeDto
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public string? Keyword { get; set; }
    }
}