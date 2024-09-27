using CategoriasMvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CategoriasMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
