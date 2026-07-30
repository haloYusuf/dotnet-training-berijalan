using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDMS.Modules.Api.Master.Dto.Response
{
    public class ResMstCustomerDto
    {
        public int Id { get; set; }
        public string Nik { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}