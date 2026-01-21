using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    public class ClienteDetalheDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public byte[]? Logotipo { get; set; }
        public IEnumerable<LogradouroDto> Logradouros { get; set; }
    }
}