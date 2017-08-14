using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeaCleaner.Domain;
using Microsoft.AspNetCore.Identity;
using SeaCleaner.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace SeaCleaner.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Player> _userManager;
        private readonly SignInManager<Player> _signInManager;        
        public AccountController(UserManager<Player> userManager, SignInManager<Player> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isPlayerLoginExists = await _userManager.Users.AnyAsync(u => u.UserName.Equals(model.Login, StringComparison.OrdinalIgnoreCase));
                if (isPlayerLoginExists)
                {
                    ModelState.AddModelError(string.Empty, "Логин занят");                    
                }
                bool isEmailExists = await _userManager.Users.AnyAsync(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
                if (isEmailExists)
                {
                    ModelState.AddModelError(string.Empty, $"Игрок с почтой \"{model.Email}\" уже зарегистрирован");
                }
                if(isEmailExists || isPlayerLoginExists)
                {
                    return View(model);
                }

                Player player = new Player { UserName = model.Login, Email = model.Email };
                var createResult = await _userManager.CreateAsync(player, model.Password);
                if (!createResult.Succeeded)
                {
                    createResult.Errors.ToList().ForEach(e=>ModelState.AddModelError(string.Empty, e.Description));
                    return View(model);
                }

                await _signInManager.SignInAsync(player, false);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid) return View(loginModel);
            var loginResulst = await _signInManager.PasswordSignInAsync(loginModel.Login, loginModel.Password, loginModel.RememberMe, false);
            if (!loginResulst.Succeeded)
            {
                ModelState.AddModelError("", "Неправильный логин и/или пароль");
                return View(loginModel);
            }
            return (string.IsNullOrWhiteSpace(loginModel.ReturnUrl) || !Url.IsLocalUrl(loginModel.ReturnUrl)) 
                ? (IActionResult)RedirectToAction("Index", "Home") 
                : Redirect(loginModel.ReturnUrl);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
