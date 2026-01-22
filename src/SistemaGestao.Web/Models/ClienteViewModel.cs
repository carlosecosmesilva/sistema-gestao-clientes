using System.ComponentModel.DataAnnotations;

namespace SistemaGestao.Web.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        public byte[]? LogotipoBytes { get; set; }

        [Display(Name = "Logotipo da Empresa")]
        public IFormFile? LogotipoUpload { get; set; }

        public List<LogradouroViewModel> Logradouros { get; set; } = new();
    }
}
