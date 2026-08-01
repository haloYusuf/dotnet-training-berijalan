using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IDMS.Web.Controllers
{
    [Authorize]
    public class VehicleDeliveryController : Controller
    {
        private readonly IVehicleDeliveryService _deliveryService;

        public VehicleDeliveryController(IVehicleDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? keyword, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _deliveryService.GetListAsync(keyword, page, limit);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            var (data, error) = await _deliveryService.GetByIdAsync(id);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] VehicleDeliveryRequestDto request)
        {
            var (data, error) = await _deliveryService.CreateAsync(request);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data, message = "Delivery created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] VehicleDeliveryRequestDto request)
        {
            var (data, error) = await _deliveryService.UpdateAsync(id, request);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data, message = "Delivery updated successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> UpdateStatus(int id, [FromBody] DeliveryStatusRequest request)
        {
            var (success, error) = await _deliveryService.UpdateStatusAsync(id, request.Status);

            if (!success) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", message = "Status updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _deliveryService.DeleteAsync(id);

            if (!success) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", message = "Delivery deleted successfully" });
        }
    }

    public class DeliveryStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}