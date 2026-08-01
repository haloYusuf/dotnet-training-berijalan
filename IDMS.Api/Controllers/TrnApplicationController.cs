using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request.TrnApplication;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDMS.Api.Controllers
{
    [ApiController]
    [Route("api/application")]
    [Authorize]
    public class TrnApplicationController : Controller
    {
        private readonly ITrnApplicationService _service;

        public TrnApplicationController(ITrnApplicationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] ReqTrnApplicationDto request)
        {
            var (data, total) = await _service.GetListAsync(request);

            return Ok(ApiResponseHelper.Success(HttpContext, data, request.Page, request.Limit, total));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            var data = await _service.GetApplicationByIdAsync(id);
            return data is null ?
                NotFound(ApiResponseHelper.Error(HttpContext, "Application Not Found", "null")) :
                Ok(ApiResponseHelper.Success(HttpContext, data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReqTrnApplicationCreateDto request)
        {
            var data = await _service.CreateAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Application created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReqTrnApplicationUpdateDto request)
        {
            var data = await _service.UpdateAsync(id, request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Application updated successfully"));
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var data = await _service.UpdateStatusAsync(id, status);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Application updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync(id);
            return Ok(ApiResponseHelper.Success(HttpContext, (object?)null, "Application deleted successfully"));
        }
    }
}