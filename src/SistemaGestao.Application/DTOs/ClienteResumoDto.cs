using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    public class ClienteResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? Logotipo { get; set; }
    }
}