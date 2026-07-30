using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Request
{
    public class ReqMstUserCreateDto
    {
        [Required(ErrorMessage ="Email is Required")]
        [EmailAddress(ErrorMessage ="Email is not Valid")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage ="Password is Required")]
        [MinLength(6, ErrorMessage ="Password min 6")]
        public string Password { get; set; } = null!;
        
        [Required(ErrorMessage ="Fullname is Required")]
        public string FullName { get; set; } = null!;
    }
}