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
    public class TypeController : Controller
    {
        private readonly ITypeService _typeService;

        public TypeController(ITypeService typeService)
        {
            _typeService = typeService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? keyword, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _typeService.GetListAsync(keyword, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null, pagination = (object?)null });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            // Memanggil method GetByIdAsync dari TypeService
            var (data, error) = await _typeService.GetByIdAsync(id);

            // Jika ada error (dari API atau koneksi), kembalikan status Error beserta pesannya
            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            // Jika sukses, kembalikan status Success beserta datanya ke Frontend
            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] TypeRequest request)
        {
            var (data, error) = await _typeService.CreateAsync(request.BrandId, request.Code, request.Name, request.Year);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Type created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] TypeRequest request)
        {
            // Teruskan request.IsActive ke service
            var (data, error) = await _typeService.UpdateAsync(id, request.BrandId, request.Code, request.Name, request.Year, request.IsActive);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Type updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken] // Tambahkan ini agar tidak kena error 400 Bad Request dari form Ajax
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _typeService.DeleteAsync(id);

            if (!success)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", message = "Type deleted successfully" });
        }
    }

    public class TypeRequest
    {
        public int BrandId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }

        // Tambahkan properti IsActive
        public bool IsActive { get; set; }
    }
}