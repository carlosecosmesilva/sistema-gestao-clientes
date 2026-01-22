using Microsoft.AspNetCore.Mvc;
using SistemaGestao.Application.DTOs;
using SistemaGestao.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace SistemaGestao.API.Controllers
{
    [Authorize] // Protege todos os endpoints por meio de JWT
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController(IClienteService service) : ControllerBase
    {
        private readonly IClienteService _service = service;

        /// <summary>
        /// Retorna todos os clientes cadastrados.
        /// </summary>
        /// <returns>Lista de clientes sem os dados do logotipo.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Retorna DTO Leve (sem imagem) -> Rápido
            var clientes = await _service.ObterTodosAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Retorna um cliente pelo ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <returns>Cliente correspondente ao ID fornecido.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _service.ObterPorIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        /// <summary>
        /// Cria um novo cliente com upload de logotipo.
        /// </summary>
        /// <param name="model">Modelo contendo os dados do cliente e o arquivo do logotipo.</param>
        /// <returns>Retorna o cliente criado com seu ID.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ClienteUploadModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dto = new ClienteCreateDto
            {
                Nome = model.Nome,
                Email = model.Email,
                Telefone = model.Telefone
            };

            // Conversão IFormFile -> byte[]
            if (model.Logotipo != null)
            {
                using var ms = new MemoryStream();
                await model.Logotipo.CopyToAsync(ms);
                dto.Logotipo = ms.ToArray();
            }

            // Adicionar Logradouros
            if (model.Logradouros != null && model.Logradouros.Count > 0)
            {
                dto.Logradouros = model.Logradouros.Select(l => new LogradouroDto
                {
                    Endereco = l.Endereco,
                    Complemento = l.Complemento,
                    Bairro = l.Bairro,
                    Cidade = l.Cidade,
                    Estado = l.Estado,
                    CEP = l.CEP
                }).ToList();
            }

            try
            {
                var result = await _service.CriarAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um cliente existente com upload de logotipo.
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado.</param>
        /// <param name="model">Modelo contendo os dados atualizados do cliente e o arquivo do logotipo.</param>
        /// <returns>Retorna NoContent em caso de sucesso.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ClienteUploadModel model)
        {
            var dto = new ClienteDetalheDto
            {
                Id = id,
                Nome = model.Nome,
                Email = model.Email,
                Telefone = model.Telefone
            };

            if (model.Logotipo != null)
            {
                using var ms = new MemoryStream();
                await model.Logotipo.CopyToAsync(ms);
                dto.Logotipo = ms.ToArray();
            }

            // Adicionar Logradouros
            if (model.Logradouros != null && model.Logradouros.Count > 0)
            {
                dto.Logradouros = model.Logradouros.Select(l => new LogradouroDto
                {
                    Id = l.Id,
                    Endereco = l.Endereco,
                    Complemento = l.Complemento,
                    Bairro = l.Bairro,
                    Cidade = l.Cidade,
                    Estado = l.Estado,
                    CEP = l.CEP
                }).ToList();
            }

            try
            {
                await _service.AtualizarAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove um cliente pelo ID.
        /// </summary>
        /// <param name="id">ID do cliente a ser removido.</param>
        /// <returns>Retorna NoContent em caso de sucesso.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.RemoverAsync(id);
            return NoContent();
        }
    }

    // Model específica para entrada na API (Binding do IFormFile)
    public class ClienteUploadModel
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public IFormFile? Logotipo { get; set; }
        public List<LogradouroUploadModel> Logradouros { get; set; } = new();
    }

    public class LogradouroUploadModel
    {
        public int Id { get; set; }
        public string Endereco { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
    }
}