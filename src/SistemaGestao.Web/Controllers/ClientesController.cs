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

        /// <summary>
        /// Exibe o formulário para criar um novo cliente.
        /// </summary>
        /// <returns>View com o formulário de criação de cliente.</returns>
        [HttpGet]
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            return View();
        }

        /// <summary>
        /// Processa o formulário de criação de um novo cliente.
        /// </summary>
        /// <param name="model">Modelo contendo os dados do cliente a ser criado.</param>
        /// <returns>Redireciona para a lista de clientes em caso de sucesso ou retorna a view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var sucesso = await _clienteService.Criar(model);

            if (sucesso)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Erro ao criar cliente na API.");
            return View(model);
        }

        /// <summary>
        /// Exibe os detalhes de um cliente específico.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>View com os detalhes do cliente.</returns>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            var cliente = await _clienteService.ObterPorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        /// <summary>
        /// Exibe o formulário para editar um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>View com o formulário de edição do cliente.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            var cliente = await _clienteService.ObterPorId(id);
            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        /// <summary>
        /// Processa o formulário de edição de um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <param name="model">Modelo contendo os dados atualizados do cliente.</param>
        /// <returns>Redireciona para a lista de clientes em caso de sucesso ou retorna a view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var sucesso = await _clienteService.Atualizar(model);

            if (sucesso)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Erro ao atualizar cliente na API.");
            return View(model);
        }

        /// <summary>
        /// Exclui um cliente existente.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>Redireciona para a lista de clientes em caso de sucesso ou retorna a view com erros.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
                return RedirectToAction("Login", "Auth");

            var sucesso = await _clienteService.Deletar(id);

            if (sucesso)
                return RedirectToAction(nameof(Index));

            TempData["Erro"] = "Erro ao excluir cliente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
