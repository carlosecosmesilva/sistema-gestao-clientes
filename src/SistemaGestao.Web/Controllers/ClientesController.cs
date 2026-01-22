using Microsoft.AspNetCore.Mvc;
using SistemaGestao.Web.Models;
using SistemaGestao.Web.Services;

namespace SistemaGestao.Web.Controllers
{
    public class ClientesController(ClienteService clienteService) : Controller
    {
        private readonly ClienteService _clienteService = clienteService;

        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            var clientes = await _clienteService.ObterTodos();
            return View(clientes);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var sucesso = await _clienteService.Criar(model);
            
            if (sucesso) 
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Erro ao criar cliente na API.");
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.ObterPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.ObterPorId(id);
            if (cliente == null) 
                return NotFound();

            await _clienteService.Atualizar(cliente);

            return View(cliente);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.ObterPorId(id);
            if (cliente == null) 
                return NotFound();

            await _clienteService.Deletar(id);
            
            return View(cliente);
        }

    }
}
