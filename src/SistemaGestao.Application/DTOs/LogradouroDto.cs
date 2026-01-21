using System.Collections.Generic;

namespace SistemaGestao.Application.DTOs
{
    public class LogradouroDto
    {
        public int Id { get; set; }
        public string Endereco { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
    }
}