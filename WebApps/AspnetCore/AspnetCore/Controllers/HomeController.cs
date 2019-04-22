using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspnetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AspnetCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IHttpContextAccessor _httpContextAccessor { get; set; }
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Accounts() {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task SignOut()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(_httpContextAccessor.HttpContext, "oidc");
        }

        [AllowAnonymous]
        public async Task SignIn()
        {
            await AuthenticationHttpContextExtensions.SignInAsync(_httpContextAccessor.HttpContext, "oidc", new ClaimsPrincipal(User.Identity));
        }
    }
}
