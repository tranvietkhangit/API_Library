using Microsoft.AspNetCore.Mvc;
using Library_Web.Models.DTO;
using System.IdentityModel.Tokens.Jwt;

namespace Library_Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsJsonAsync(
                "https://localhost:7045/api/User/Login",
                model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "Username hoặc Password không đúng");

                return View(model);
            }

            var result = await response.Content
    .ReadFromJsonAsync<LoginResponseDTO>();

            if (result == null || string.IsNullOrEmpty(result.JwtToken))
            {
                ModelState.AddModelError(
                    "",
                    "Không nhận được JWT từ API");

                return View(model);
            }

            HttpContext.Session.SetString(
    "JWT",
    result.JwtToken);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.ReadJwtToken(result.JwtToken);

            var role = token.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                ?.Value;

            if (!string.IsNullOrEmpty(role))
            {
                HttpContext.Session.SetString("Role", role);
            }

            return RedirectToAction("Index", "Books");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }
    }
}
