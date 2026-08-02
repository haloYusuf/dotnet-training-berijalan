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
    public class ApplicationController : Controller
    {
        private readonly IApplicationService _appService;

        public ApplicationController(IApplicationService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? keyword, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _appService.GetListAsync(keyword, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> ApprovedList()
        {
            var (data, error) = await _appService.GetApprovedListAsync();

            if (error != null)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", data});
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            var (data, error) = await _appService.GetByIdAsync(id);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] ApplicationRequestDto request)
        {
            var (data, error) = await _appService.CreateAsync(request);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data, message = "Application created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] ApplicationRequestDto request)
        {
            var (data, error) = await _appService.UpdateAsync(id, request);

            if (error != null) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", data, message = "Application updated successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> UpdateStatus(int id, [FromBody] ApplicationStatusRequest request)
        {
            var (success, error) = await _appService.UpdateStatusAsync(id, request.Status);

            if (!success) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", message = "Status updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _appService.DeleteAsync(id);

            if (!success) return Json(new { status = "Error", message = error });
            return Json(new { status = "Success", message = "Application deleted successfully" });
        }
    }

    public class ApplicationStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}