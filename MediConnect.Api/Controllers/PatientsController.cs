using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Api.Controllers
{
    public class PatientsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
