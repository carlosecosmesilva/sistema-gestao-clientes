using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    public class ClienteCreateDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public byte[]? Logotipo { get; set; }
        public ICollection<LogradouroDto> Logradouros { get; set; } = [];
    }
}