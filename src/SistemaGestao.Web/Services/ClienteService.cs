using SistemaGestao.Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SistemaGestao.Web.Services
{
    public class ClienteService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        // Método auxiliar para pegar o Token da Sessão
        private void AdicionarToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<ClienteViewModel>> ObterTodos()
        {
            AdicionarToken();
            var response = await _httpClient.GetAsync("clientes");
            if (!response.IsSuccessStatusCode) return new List<ClienteViewModel>();

            var clientes = await response.Content.ReadFromJsonAsync<List<ClienteViewModel>>();

            // Converte bytes para Base64 para exibição
            foreach (var cliente in clientes ?? [])
            {
                if (cliente.LogotipoBytes != null && cliente.LogotipoBytes.Length > 0)
                    cliente.LogotipoBase64 = Convert.ToBase64String(cliente.LogotipoBytes);
            }

            return clientes ?? [];
        }

        public async Task<ClienteViewModel?> ObterPorId(int id)
        {
            AdicionarToken();
            var response = await _httpClient.GetAsync($"clientes/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var cliente = await response.Content.ReadFromJsonAsync<ClienteViewModel>();

            // Converte bytes para Base64
            if (cliente?.LogotipoBytes != null && cliente.LogotipoBytes.Length > 0)
                cliente.LogotipoBase64 = Convert.ToBase64String(cliente.LogotipoBytes);

            return cliente;
        }

        public async Task<bool> Criar(ClienteViewModel model)
        {
            AdicionarToken();
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Nome), "Nome");
            content.Add(new StringContent(model.Email), "Email");

            if (model.LogotipoUpload != null)
            {
                var fileStream = model.LogotipoUpload.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.LogotipoUpload.ContentType);
                content.Add(fileContent, "Logotipo", model.LogotipoUpload.FileName);
            }

            var response = await _httpClient.PostAsync("clientes", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Atualizar(ClienteViewModel model)
        {
            AdicionarToken();
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Nome), "Nome");
            content.Add(new StringContent(model.Email), "Email");

            if (model.LogotipoUpload != null)
            {
                var fileStream = model.LogotipoUpload.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.LogotipoUpload.ContentType);
                content.Add(fileContent, "Logotipo", model.LogotipoUpload.FileName);
            }

            var response = await _httpClient.PutAsync($"clientes/{model.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Deletar(int id)
        {
            AdicionarToken();
            var response = await _httpClient.DeleteAsync($"clientes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
