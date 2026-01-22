using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    // Resultado simples para criação de cliente
    public class ClienteResultDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
    }
}