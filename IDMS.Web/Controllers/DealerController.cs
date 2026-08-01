using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDMS.Web.Controllers
{
    [Authorize]
    public class DealerController : Controller
    {
        private readonly IDealerService _dealerService;

        public DealerController(IDealerService dealerService)
        {
            _dealerService = dealerService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? keyword, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _dealerService.GetListAsync(keyword, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null, pagination = (object?)null });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            var (data, error) = await _dealerService.GetByIdAsync(id);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] DealerRequestDto request)
        {
            var (data, error) = await _dealerService.CreateAsync(request);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Dealer created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] DealerRequestDto request)
        {
            var (data, error) = await _dealerService.UpdateAsync(id, request);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Dealer updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _dealerService.DeleteAsync(id);

            if (!success)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", message = "Dealer deleted successfully" });
        }
    }
}