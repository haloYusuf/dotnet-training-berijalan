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
    public class InsuranceController : Controller
    {
        private readonly IInsuranceService _insuranceService;

        public InsuranceController(IInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? keyword, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _insuranceService.GetListAsync(keyword, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null, pagination = (object?)null });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            var (data, error) = await _insuranceService.GetByIdAsync(id);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] InsuranceRequestDto request)
        {
            var (data, error) = await _insuranceService.CreateAsync(request);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Insurance created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] InsuranceRequestDto request)
        {
            var (data, error) = await _insuranceService.UpdateAsync(id, request);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Insurance updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _insuranceService.DeleteAsync(id);

            if (!success)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", message = "Insurance deleted successfully" });
        }
    }
}