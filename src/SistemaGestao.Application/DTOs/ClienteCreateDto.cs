using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    public class ClienteCreateDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public byte[]? Logotipo { get; set; }
        public ICollection<LogradouroDto> Logradouros { get; set; }
    }
}