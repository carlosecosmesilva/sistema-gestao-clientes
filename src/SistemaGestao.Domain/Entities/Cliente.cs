using System.Collections.Generic;

namespace SistemaGestao.Domain.Entities
{
    public class Cliente
    {
        public Cliente()
        {
            Logradouros = new List<Logradouro>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public byte[]? Logotipo { get; set; } // O arquivo em si (blob)

        // Relacionamento 1:N
        public ICollection<Logradouro> Logradouros { get; set; }
    }
}