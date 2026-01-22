using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace SistemaGestao.API.IntegrationTests
{
    /// <summary>
    /// Fornece testes de integração para os endpoints da API ClientesController, verificando autenticação, 
    /// operações CRUD e respostas HTTP esperadas usando uma instância de servidor de teste.
    /// </summary>
    /// <remarks>Esses testes abrangem cenários como requisitos de autenticação, 
    /// tratamento de dados válidos e inválidos e códigos de resposta para o ClientesController. 
    /// A classe de teste usa um cliente HTTP real contra um servidor de teste, 
    /// garantindo que a API se comporte conforme o esperado em cenários de ponta a ponta. 
    /// Os testes são isolados e não exigem dependências externas
    /// além do servidor de teste.</remarks>
    /// <param name="factory">A instância WebApplicationFactory usada para criar um servidor de teste em memória para a API em teste.</param>
    public class ClientesControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        #region Autenticação

        [Fact]
        public async Task GetClientes_SemToken_DeveRetornarUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/clientes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PostLogin_ComCredenciaisValidas_DeveRetornarOkComToken()
        {
            // Arrange
            var loginData = new { Username = "admin", Password = "admin" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("token");
        }

        [Fact]
        public async Task PostLogin_ComCredenciaisInvalidas_DeveRetornarUnauthorized()
        {
            // Arrange
            var loginData = new { Username = "admin", Password = "senhaErrada" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region GET /api/clientes

        [Fact]
        public async Task GetClientes_ComTokenValido_DeveRetornarOk()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/clientes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region GET /api/clientes/{id}

        [Fact]
        public async Task GetClientePorId_ComIdValido_DeveRetornarOk()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Primeiro, criar um cliente
            var novoCliente = await CriarClienteAsync(token, "Empresa Teste GET", "testeget@empresa.com");
            var clienteId = novoCliente.id;

            // Act
            var response = await _client.GetAsync($"/api/clientes/{clienteId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultado = await response.Content.ReadFromJsonAsync<dynamic>();
            resultado.Should().NotBeNull();
            ((int)resultado.id).Should().Be(clienteId);
        }

        [Fact]
        public async Task GetClientePorId_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/clientes/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region POST /api/clientes (Criar)

        [Fact]
        public async Task PostCriarCliente_ComLogotipo_DeveRetornarCreated()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Preparar dados do formulário (Multipart)
            using var content = new MultipartFormDataContent
            {
                { new StringContent("Empresa Teste Integração"), "Nome" },
                { new StringContent("suporte@testeintegracao.com"), "Email" }
            };

            // Simulação do arquivo de imagem (Logotipo)
            var bytesImagem = Encoding.UTF8.GetBytes("conteudo-falso-de-imagem-png");
            var fileContent = new ByteArrayContent(bytesImagem);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            content.Add(fileContent, "Logotipo", "logo_empresa.png");

            // Act
            var response = await _client.PostAsync("/api/clientes", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var resultado = await response.Content.ReadFromJsonAsync<dynamic>();
            resultado.Should().NotBeNull();
            ((int)resultado.id).Should().BeGreaterThan(0);
            ((string)resultado.nome).Should().Be("Empresa Teste Integração");
        }

        [Fact]
        public async Task PostCriarCliente_SemDados_DeveRetornarBadRequest()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            // Act
            var response = await _client.PostAsync("/api/clientes", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostCriarCliente_ComEmailDuplicado_DeveRetornarBadRequest()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var emailDuplicado = "duplicado@teste.com";

            // Criar primeiro cliente
            await CriarClienteAsync(token, "Empresa 1", emailDuplicado);

            // Tentar criar com mesmo email
            using var content = new MultipartFormDataContent
            {
                { new StringContent("Empresa 2"), "Nome" },
                { new StringContent(emailDuplicado), "Email" }
            };

            // Act
            var response = await _client.PostAsync("/api/clientes", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().Contain("e-mail já está cadastrado");
        }

        [Fact]
        public async Task PostCriarCliente_SemToken_DeveRetornarUnauthorized()
        {
            // Arrange
            using var content = new MultipartFormDataContent
            {
                { new StringContent("Empresa Teste"), "Nome" },
                { new StringContent("teste@empresa.com"), "Email" }
            };

            // Act
            var response = await _client.PostAsync("/api/clientes", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region PUT /api/clientes/{id} (Atualizar)

        [Fact]
        public async Task PutAtualizarCliente_ComDadosValidos_DeveRetornarNoContent()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Criar cliente
            var novoCliente = await CriarClienteAsync(token, "Empresa Original", "original@teste.com");
            var clienteId = novoCliente.id;

            // Preparar dados para atualização
            using var updateContent = new MultipartFormDataContent
            {
                { new StringContent("Empresa Atualizada"), "Nome" },
                { new StringContent("atualizada@teste.com"), "Email" }
            };

            // Act
            var response = await _client.PutAsync($"/api/clientes/{clienteId}", updateContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task PutAtualizarCliente_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent
            {
                { new StringContent("Empresa"), "Nome" },
                { new StringContent("teste@teste.com"), "Email" }
            };

            // Act
            var response = await _client.PutAsync("/api/clientes/99999", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        #endregion

        #region DELETE /api/clientes/{id}

        [Fact]
        public async Task DeleteCliente_ComIdValido_DeveRetornarNoContent()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Criar cliente
            var novoCliente = await CriarClienteAsync(token, "Empresa Para Deletar", "deleta@teste.com");
            var clienteId = novoCliente.id;

            // Act
            var response = await _client.DeleteAsync($"/api/clientes/{clienteId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar que foi realmente deletado
            var getResponse = await _client.GetAsync($"/api/clientes/{clienteId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCliente_ComIdInvalido_DeveRetornarNotFound()
        {
            // Arrange
            var token = await ObterTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync("/api/clientes/99999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCliente_SemToken_DeveRetornarUnauthorized()
        {
            // Act
            var response = await _client.DeleteAsync("/api/clientes/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Métodos Auxiliares

        private async Task<string> ObterTokenAsync()
        {
            var loginData = new { Username = "admin", Password = "admin" };
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginData);
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (result is null || string.IsNullOrEmpty(result.Token))
                throw new Xunit.Sdk.XunitException("Token de autenticação não foi retornado.");
            return result.Token;
        }

        private async Task<dynamic> CriarClienteAsync(string token, string nome, string email)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent
            {
                { new StringContent(nome), "Nome" },
                { new StringContent(email), "Email" }
            };

            var response = await _client.PostAsync("/api/clientes", content);
            if (!response.IsSuccessStatusCode)
                throw new Xunit.Sdk.XunitException($"Falha ao criar cliente: {response.StatusCode}");

            return await response.Content.ReadFromJsonAsync<dynamic>();
        }

        public class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        #endregion
    }
}
