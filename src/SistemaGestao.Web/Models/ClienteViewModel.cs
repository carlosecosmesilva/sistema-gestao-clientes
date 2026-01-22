using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SistemaGestao.Web.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        public string? Telefone { get; set; }

        [JsonPropertyName("Logotipo")]
        public byte[]? LogotipoBytes { get; set; }

        [JsonIgnore]
        public string? LogotipoBase64 { get; set; }

        [JsonIgnore]
        [Display(Name = "Logotipo da Empresa")]
        public IFormFile? LogotipoUpload { get; set; }

        public List<LogradouroViewModel> Logradouros { get; set; } = new();
    }
}
