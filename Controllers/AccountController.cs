using ALLINONEPROJECTWITHOUTJS.Data;
using ALLINONEPROJECTWITHOUTJS.DTOs;
using ALLINONEPROJECTWITHOUTJS.Models;
using ALLINONEPROJECTWITHOUTJS.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace ALLINONEPROJECTWITHOUTJS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;
        private readonly IAuthRepository _authRepository;
        public AccountController(AppDbContext context, IConfiguration configuration, IAuthRepository authRepository)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("ConnectionString");
            _authRepository = authRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var response = await _authRepository.LoginAsync(request);

            if (response == 0)
            {
                ViewBag.Message = "Invalid username or password.";
                return View();
            }
            else
                return RedirectToAction("Item", "Item");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _authRepository.RegisterAsync(request);

            if (response == 0)
                return Json("User already exists!");
            else
                return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _authRepository.ForgotPasswordAsync(email);
            if (user != null)
            {
                return Json($"Reset link sent to {email}");
            }
            else
                return Json("Email not found");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();    
            return RedirectToAction("Login");
        }
    }
}
