using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaGestao.Application.DTOs;

namespace SistemaGestao.Application.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteResumoDto>> ObterTodosAsync();
        Task<ClienteDetalheDto> ObterPorIdAsync(int id);
        Task<ClienteResultDto> CriarAsync(ClienteCreateDto dto);
        Task AtualizarAsync(ClienteDetalheDto dto);
        Task RemoverAsync(int id);
    }
}