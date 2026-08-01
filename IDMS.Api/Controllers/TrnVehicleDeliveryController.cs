using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Api.Helpers;
using IDMS.Modules.Api.Master.Dto.Request.TrnVehicleDelivery;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDMS.Api.Controllers
{
    [ApiController]
    [Route("api/vehicleDeliv")]
    [Authorize]
    public class TrnVehicleDeliveryController : Controller
    {
        private readonly ITrnVehicleDeliveryService _service;

        public TrnVehicleDeliveryController(ITrnVehicleDeliveryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] ReqTrnVehicleDeliveryDto request)
        {
            var (data, total) = await _service.GetListAsync(request);

            return Ok(ApiResponseHelper.Success(HttpContext, data, request.Page, request.Limit, total));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getById(int id)
        {
            var data = await _service.GetVehicleDeliveryByIdAsync(id);
            return data is null ?
                NotFound(ApiResponseHelper.Error(HttpContext, "Vehicle Delivery Not Found", "null")) :
                Ok(ApiResponseHelper.Success(HttpContext, data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReqTrnVehicleDeliveryCreateDto request)
        {
            var data = await _service.CreateAsync(request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Vehicle Delivery created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReqTrnVehicleDeliveryUpdateDto request)
        {
            var data = await _service.UpdateAsync(id, request);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Vehicle Delivery updated successfully"));
        }

        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var data = await _service.UpdateStatusAsync(id, status);
            return Ok(ApiResponseHelper.Success(HttpContext, data, "Vehicle Delivery updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _service.SoftDeleteAsync(id);
            return Ok(ApiResponseHelper.Success(HttpContext, (object?)null, "Vehicle Delivery deleted successfully"));
        }
    }
}