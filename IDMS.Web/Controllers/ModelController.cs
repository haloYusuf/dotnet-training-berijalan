using IDMS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IDMS.Web.Controllers
{
    [Authorize]
    public class ModelController : Controller
    {
        private readonly IModelService _modelService;

        public ModelController(IModelService modelService)
        {
            _modelService = modelService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? keyword, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _modelService.GetListAsync(keyword, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null, pagination = (object?)null });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            // Memanggil method GetByIdAsync dari TypeService
            var (data, error) = await _modelService.GetByIdAsync(id);

            // Jika ada error (dari API atau koneksi), kembalikan status Error beserta pesannya
            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            // Jika sukses, kembalikan status Success beserta datanya ke Frontend
            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] ModelRequest request)
        {
            var (data, error) = await _modelService.CreateAsync(request.TypeId, request.Code, request.Name, request.Year, request.Price, request.Stock);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Type created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] ModelRequest request)
        {
            var (data, error) = await _modelService.UpdateAsync(id, request.TypeId, request.Code, request.Name, request.Year, request.Price, request.Stock, request.IsActive);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Type updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _modelService.DeleteAsync(id);

            if (!success)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", message = "Type deleted successfully" });
        }
    }

    public class ModelRequest
    {
        public int TypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Tambahkan properti IsActive
        public bool IsActive { get; set; }
    }
}
