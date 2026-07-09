using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Api.Controllers
{
    public class VitalsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
