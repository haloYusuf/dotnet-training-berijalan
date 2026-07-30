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
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customer;

        public CustomerController(ICustomerService customer)
        {
            _customer = customer;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> List(string? search, int page = 1, int limit = 10)
        {
            var (data, pagination, error) = await _customer.GetListAsync(search, page, limit);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null, pagination = (object?)null });

            return Json(new { status = "Success", data, pagination });
        }

        [HttpGet]
        public async Task<JsonResult> Detail(int id)
        {
            var (data, error) = await _customer.GetByIdAsync(id);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Create([FromBody] CustomerRequest request)
        {
            var (data, error) = await _customer.CreateAsync(request.Nik, request.FullName, request.BirthDate, request.Phone, request.Email, request.Address);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Brand created successfully" });
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Update(int id, [FromBody] CustomerRequest request)
        {
            var (data, error) = await _customer.UpdateAsync(id, request.Nik, request.FullName, request.BirthDate, request.Phone, request.Email, request.Address);

            if (error != null)
                return Json(new { status = "Error", message = error, data = (object?)null });

            return Json(new { status = "Success", data, message = "Brand updated successfully" });
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var (success, error) = await _customer.DeleteAsync(id);

            if (!success)
                return Json(new { status = "Error", message = error });

            return Json(new { status = "Success", message = "Brand deleted successfully" });
        }
    }

    public class CustomerRequest
    {
        public string Nik { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? BirthDate { get; set; } = DateTime.Now;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}