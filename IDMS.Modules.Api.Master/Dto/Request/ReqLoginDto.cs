using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace IDMS.Modules.Api.Master.Dto.Request
{
    public class ReqLoginDto
    {
        public string Email {get;set;} = null!;
        public string Password {get;set;} = null!;
    }
}