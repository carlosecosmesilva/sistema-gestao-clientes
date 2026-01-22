using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    public class ClienteDetalheDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public byte[]? Logotipo { get; set; }
        public IEnumerable<LogradouroDto> Logradouros { get; set; } = [];
    }
}