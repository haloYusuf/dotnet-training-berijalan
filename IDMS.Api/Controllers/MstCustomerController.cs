using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Mvc;

namespace IDMS.Api.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class MstCustomerController : ControllerBase
    {
        private readonly IMstCustomerService _service;

        public MstCustomerController(IMstCustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] ReqMstCustomerDto request)
        {
            var (data, total) = await _service.GetListAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, request.Page, request.Limit, total));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            var data = await _service.GetCustomerByIdAsync(id);
            return data is null ?
                NotFound(ApiResponseHelper.Error(HttpContext, "Customer Not Found", "null")) :
                Ok(ApiResponseHelper.Success(HttpContext, data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReqMstCustomerCreateDto request)
        {
            var data = await _service.CreateAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Customer created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReqMstCustomerUpdateDto request)
        {
            var data = await _service.UpdateAsync(id, request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Customer updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync(id);
            return Ok(ApiResponseHelper.Success(HttpContext, (object?)null, "Customer deleted successfully"));
        }
    }
}