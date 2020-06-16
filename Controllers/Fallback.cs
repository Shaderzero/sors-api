using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace sors.Controllers
{
    public class Fallback : Controller
    {
        private readonly IConfiguration _configuration;

        public Fallback(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var path = _configuration[HostDefaults.ContentRootKey];
            return PhysicalFile(Path.Combine(path, "wwwroot", "index.html"), "text/HTML");
        }
    }
}
