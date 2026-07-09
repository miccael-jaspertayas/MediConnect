using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Api.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
