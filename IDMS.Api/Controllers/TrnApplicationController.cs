using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IDMS.Modules.Api.Master.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDMS.Api.Controllers
{
    [Route("[controller]")]
    public class TrnApplicationController : Controller
    {
        private readonly ITrnApplicationService _service;

        public TrnApplicationController(ITrnApplicationService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}