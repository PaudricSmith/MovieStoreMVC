using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieStoreMVC.Models.DTO;
using MovieStoreMVC.Repositories.Abstract;

namespace MovieStoreMVC.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private IUserAuthenticationService _userAuthenticationService;

        public UserAuthenticationController(IUserAuthenticationService userAuthenticationService)
        {
            _userAuthenticationService = userAuthenticationService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Role = "user";

            var result = await _userAuthenticationService.RegisterAsync(model);
            TempData["msg"] = result.Message;

            return RedirectToAction(nameof(Register));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) 
                return View(model);

            var result = await _userAuthenticationService.LoginAsync(model);
            if (result.StatusCode == 1)
                return RedirectToAction("Index", "Home");
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userAuthenticationService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userAuthenticationService.ChangePasswordAsync(model, User.Identity.Name);
            TempData["msg"] = result.Message;

            return RedirectToAction(nameof(ChangePassword));
        }

        //public async Task<IActionResult> InitAdmin()
        //{
        //    var model = new RegistrationModel
        //    {
        //        UserName = "admin1",
        //        Name = "Paudric",
        //        Email = "paudric@gmail.com",
        //        Password = "Admin@123"
        //    };

        //    model.Role = "admin";
        //    var result = await _userAuthenticationService.RegisterAsync(model);
        //    return Ok(result);
        //}
    }
}
