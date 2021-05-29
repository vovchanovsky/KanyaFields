using System.Threading.Tasks;
using ApiSvc.InfrastructureInterfaces.Clients.IdentityProviderClient;
using Microsoft.AspNetCore.Mvc;

namespace ApiSvc.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(IIdentityProviderClient identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Login functionality

            var token = await _identityProvider.Authenticate(username, password);
            return Ok(new { access_token = token });
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            // Register functionality

            await _identityProvider.RegisterUser(username, password);
            return RedirectToAction("Login");
        }


        private readonly IIdentityProviderClient _identityProvider;
    }
}