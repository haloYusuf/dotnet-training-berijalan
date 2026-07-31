using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request.MstDealer;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDMS.Api.Controllers
{
    [ApiController]
    [Route("api/dealer")]
    [Authorize]
    public class DealerController : Controller
    {
        private readonly IMstDealerService _service;

        public DealerController(IMstDealerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] ReqMstDealerDto request)
        {
            var (data, total) = await _service.GetListAsync(request);

            return Ok(ApiResponseHelper.Success(HttpContext, data, request.Page, request.Limit, total));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            var data = await _service.GetDealerByIdAsync(id);
            return data is null ?
                NotFound(ApiResponseHelper.Error(HttpContext, "Brand Not Found", "null")) :
                Ok(ApiResponseHelper.Success(HttpContext, data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReqMstDealerCreateDto request)
        {
            var data = await _service.CreateAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Brand created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReqMstDealerUpdateDto request)
        {
            var data = await _service.UpdateAsync(id, request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Brand updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync(id);
            return Ok(ApiResponseHelper.Success(HttpContext, (object?)null, "Brand deleted successfully"));
        }
    }
}