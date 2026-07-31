using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request.MstType;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IDMS.Api.Controllers
{
    [ApiController]
    [Route("api/type")]
    [Authorize]
    public class MstTypeController : ControllerBase
    {
        private readonly IMstTypeService _service;

        public MstTypeController(IMstTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] ReqMstTypeDto request)
        {
            var (data, total) = await _service.GetListAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, request.Page, request.Limit, total));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            var data = await _service.GetTypeByIdAsync(id);
            return data is null ?
                NotFound(ApiResponseHelper.Error(HttpContext, "Brand Not Found", "null")) :
                Ok(ApiResponseHelper.Success(HttpContext, data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReqMstTypeCreateDto request)
        {
            var data = await _service.CreateAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Type created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReqMstTypeUpdateDto request)
        {
            var data = await _service.UpdateAsync(id, request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Type updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync(id);
            return Ok(ApiResponseHelper.Success(HttpContext, (object?)null, "Type deleted successfully"));
        }
    }
}