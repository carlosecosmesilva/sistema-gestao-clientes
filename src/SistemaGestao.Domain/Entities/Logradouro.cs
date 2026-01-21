namespace SistemaGestao.Domain.Entities
{
    public class Logradouro
    {
        public int Id { get; set; }
        public int ClienteId { get; set; } // Chave Estrangeira
        public string Endereco { get; set; }
        public string? Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }

        // Propriedade de Navegação (para o EF Core entender o vínculo)
        public Cliente Cliente { get; set; }
    }
}