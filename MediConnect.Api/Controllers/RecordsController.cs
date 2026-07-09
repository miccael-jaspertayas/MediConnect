using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Api.Controllers
{
    public class RecordsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
