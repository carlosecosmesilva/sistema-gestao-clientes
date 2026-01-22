using Microsoft.AspNetCore.Mvc;

namespace SistemaGestao.Web.Controllers
{
    public class AuthController(IHttpClientFactory clientFactory, IConfiguration configuration) : Controller
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _clientFactory.CreateClient();
            var baseUrl = _configuration["ApiSettings:BaseUrl"];

            var response = await client.PostAsJsonAsync($"{baseUrl}auth/login", new { Username = username, Password = password });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResult>();
                HttpContext.Session.SetString("JWToken", result.Token);
                return RedirectToAction("Index", "Clientes");
            }

            ViewBag.Erro = "Login inválido";
            return View();
        }
    }

    public class TokenResult { public string Token { get; set; } }
}