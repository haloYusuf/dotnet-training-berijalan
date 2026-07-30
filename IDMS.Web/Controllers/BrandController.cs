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
    public class BrandController : Controller
    {
        private readonly IBrandService _brand;

        public BrandController(IBrandService brand)
        {
            _brand = brand;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? search, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _brand.GetListAsync(search, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null, pagination = (object?)null });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            var (data, error) = await _brand.GetByIdAsync(id);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] BrandRequest request)
        {
            var (data, error) = await _brand.CreateAsync(request.Code, request.Name);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Brand created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] BrandRequest request)
        {
            var (data, error) = await _brand.UpdateAsync(id, request.Code, request.Name);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Brand updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _brand.DeleteAsync(id);

            if (!success)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", message = "Brand deleted successfully" });
        }
    }

    public class BrandRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}