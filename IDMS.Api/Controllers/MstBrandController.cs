using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request.MstBrand;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IDMS.Api.Controllers;


// [Authorize(Roles = "customer")]
[ApiController]
[Route("api/brand")]
[Authorize]
public class MstBrandController : ControllerBase
{
    private readonly IMstBrandService _service;

    public MstBrandController(IMstBrandService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] ReqMstBrandDto request)
    {
        var (data, total) = await _service.GetListAsync(request);

        return Ok(ApiResponseHelper.Success(HttpContext, data, request.Page, request.Limit, total));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> getById(int id)
    {
        var data = await _service.GetBrandByIdAsync(id);
        return data is null ?
            NotFound(ApiResponseHelper.Error(HttpContext, "Brand Not Found", "null")) :
            Ok(ApiResponseHelper.Success(HttpContext, data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReqMstBrandCreateDto request)
    {
        var data = await _service.CreateAsync(request);
        return Ok(ApiResponseHelper.Success(HttpContext, data, "Brand created successfully"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReqMstBrandUpdateDto request)
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
