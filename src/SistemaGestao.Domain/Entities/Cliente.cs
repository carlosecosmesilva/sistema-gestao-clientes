using System.Collections.Generic;

namespace SistemaGestao.Domain.Entities
{
    public class Cliente
    {
        public Cliente()
        {
            Logradouros = [];
            Nome = string.Empty;
            Email = string.Empty;
        }

        public Cliente(string nome, string email)
        {
            Nome = nome;
            Email = email;
            Logradouros = [];
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public byte[]? Logotipo { get; set; } // O arquivo em si (blob)

        // Relacionamento 1:N
        public ICollection<Logradouro> Logradouros { get; set; }
    }
}