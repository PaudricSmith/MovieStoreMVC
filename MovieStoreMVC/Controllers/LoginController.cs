using Microsoft.AspNetCore.Mvc;

namespace MovieStoreMVC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
