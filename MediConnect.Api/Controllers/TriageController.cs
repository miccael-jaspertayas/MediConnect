using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Api.Controllers
{
    public class TriageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
