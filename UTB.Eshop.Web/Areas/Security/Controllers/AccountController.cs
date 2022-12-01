using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UTB.Eshop.Web.Controllers;
using UTB.Eshop.Web.Models.ApplicationServices.Abstraction;
using UTB.Eshop.Web.Models.ViewModels;

namespace UTB.Eshop.Web.Areas.Security.Controllers
{
    [Area("Security")]
    public class AccountController : Controller
    {
        ISecurityApplicationService _securityService;
        public AccountController(ISecurityApplicationService securityService)
        {
            _securityService = securityService;
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                string[] errors = await _securityService.Register(registerVM, Models.Identity.Roles.Customer);
                if (errors == null)
                {
                    LoginViewModel loginVM = new LoginViewModel()
                    {
                        Username = registerVM.Username,
                        Password = registerVM.Password
                    };

                    return await Login(loginVM);
                }
                else
                {
                    //logovani chyby do souboru, databaze apod.
                }
            }

            return View(registerVM);
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                bool isLogged = await _securityService.Login(loginVM);
                if (isLogged)
                {
                    return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", String.Empty), new { area = String.Empty });
                }
                loginVM.LoginFailed = true;
            }

            return View(loginVM);
        }


        public async Task<IActionResult> Logout()
        {
            await _securityService.Logout();
            return RedirectToAction(nameof(Login));
        }

    }
}
