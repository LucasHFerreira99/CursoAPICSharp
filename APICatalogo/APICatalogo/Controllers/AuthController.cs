using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
