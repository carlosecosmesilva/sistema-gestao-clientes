using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaGestao.Domain.Entities;

namespace SistemaGestao.Domain.Interfaces
{
    public interface IClienteRepository
    {
        // Leitura (Queries)
        Task<IEnumerable<Cliente>> ObterTodosAsync();
        Task<Cliente?> ObterPorIdAsync(int id);
        Task<bool> ExisteEmailAsync(string email);

        // Escrita (Commands)
        Task<Cliente> AdicionarAsync(Cliente cliente);
        Task AtualizarAsync(Cliente cliente);
        Task RemoverAsync(int id);
    }
}