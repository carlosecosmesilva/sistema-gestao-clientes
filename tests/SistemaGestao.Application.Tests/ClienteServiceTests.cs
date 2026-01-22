using Moq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FluentAssertions;
using SistemaGestao.Application.DTOs;
using SistemaGestao.Application.Services;
using SistemaGestao.Domain.Entities;
using SistemaGestao.Domain.Interfaces;

namespace SistemaGestao.Application.Tests
{
    /// <summary>
    /// Fornece testes unitários para o ClienteService, 
    /// verificando seu comportamento na criação, recuperação, atualização e remoção de entidades Cliente.
    /// </summary>
    /// <remarks>Esses testes abrangem cenários de sucesso e de erro, 
    /// incluindo falhas de validação e tentativas de operar em clientes inexistentes. 
    /// A classe usa dependências simuladas para isolar a lógica do serviço e
    /// garantir resultados de teste repetíveis e confiáveis.</remarks>
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _repositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IValidator<Cliente>> _validatorMock = new();
        private readonly ClienteService _service;

        public ClienteServiceTests()
        {
            _service = new ClienteService(_repositoryMock.Object, _mapperMock.Object, _validatorMock.Object);
        }

        #region Criar Cliente - Happy Path

        [Fact]
        public async Task CriarAsync_ComDadosValidos_DeveCriarClienteComSucesso()
        {
            // Arrange
            var dto = new ClienteCreateDto { Nome = "Empresa Teste", Email = "novo@teste.com" };
            var cliente = new Cliente(dto.Nome, dto.Email);
            var clienteCriado = new Cliente(dto.Nome, dto.Email) { Id = 1 };
            var resultado = new ClienteResultDto { Id = 1, Nome = dto.Nome };

            _mapperMock.Setup(m => m.Map<Cliente>(dto)).Returns(cliente);
            _validatorMock.Setup(v => v.ValidateAsync(cliente, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.ExisteEmailAsync(dto.Email)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.AdicionarAsync(cliente)).ReturnsAsync(clienteCriado);

            // Act
            var result = await _service.CriarAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Nome.Should().Be(dto.Nome);

            // Verifica se as dependências foram chamadas
            _mapperMock.Verify(m => m.Map<Cliente>(dto), Times.Once);
            _validatorMock.Verify(v => v.ValidateAsync(cliente, default), Times.Once);
            _repositoryMock.Verify(r => r.ExisteEmailAsync(dto.Email), Times.Once);
            _repositoryMock.Verify(r => r.AdicionarAsync(cliente), Times.Once);
        }

        #endregion

        #region Criar Cliente - Erro Path

        [Fact]
        public async Task CriarAsync_ComEmailJaExistente_DeveLancarExcecao()
        {
            // Arrange
            var dto = new ClienteCreateDto { Nome = "Teste", Email = "ja_existe@teste.com" };
            var cliente = new Cliente(dto.Nome, dto.Email);

            _mapperMock.Setup(m => m.Map<Cliente>(dto)).Returns(cliente);
            _validatorMock.Setup(v => v.ValidateAsync(cliente, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.ExisteEmailAsync(dto.Email)).ReturnsAsync(true);

            // Act
            Func<Task> act = async () => await _service.CriarAsync(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Este e-mail já está cadastrado.");

            // Não deve ter chamado AdicionarAsync
            _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_ComValidacaoDomInioFalhando_DeveLancarExcecao()
        {
            // Arrange
            var dto = new ClienteCreateDto { Nome = "", Email = "email@invalido" };
            var cliente = new Cliente(dto.Nome, dto.Email);
            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Nome", "Nome não pode estar vazio")
            });

            _mapperMock.Setup(m => m.Map<Cliente>(dto)).Returns(cliente);
            _validatorMock.Setup(v => v.ValidateAsync(cliente, default))
                .ReturnsAsync(validationResult);

            // Act
            Func<Task> act = async () => await _service.CriarAsync(dto);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();
            _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        #endregion

        #region Obter Todos os Clientes

        [Fact]
        public async Task ObterTodosAsync_ComClientesCadastrados_DeveRetornarLista()
        {
            // Arrange
            var clientes = new List<Cliente>
            {
                new Cliente("Empresa 1", "contato1@teste.com") { Id = 1 },
                new Cliente("Empresa 2", "contato2@teste.com") { Id = 2 }
            };
            var dtos = new List<ClienteResumoDto>
            {
                new ClienteResumoDto { Id = 1, Nome = "Empresa 1", Email = "contato1@teste.com" },
                new ClienteResumoDto { Id = 2, Nome = "Empresa 2", Email = "contato2@teste.com" }
            };

            _repositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(clientes);
            _mapperMock.Setup(m => m.Map<IEnumerable<ClienteResumoDto>>(clientes)).Returns(dtos);

            // Act
            var result = await _service.ObterTodosAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Nome.Should().Be("Empresa 1");

            _repositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<ClienteResumoDto>>(clientes), Times.Once);
        }

        [Fact]
        public async Task ObterTodosAsync_SemClientesCadastrados_DeveRetornarListaVazia()
        {
            // Arrange
            var clientes = new List<Cliente>();
            var dtos = new List<ClienteResumoDto>();

            _repositoryMock.Setup(r => r.ObterTodosAsync()).ReturnsAsync(clientes);
            _mapperMock.Setup(m => m.Map<IEnumerable<ClienteResumoDto>>(clientes)).Returns(dtos);

            // Act
            var result = await _service.ObterTodosAsync();

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Obter Cliente por ID

        [Fact]
        public async Task ObterPorIdAsync_ComIdValido_DeveRetornarClienteDetalhe()
        {
            // Arrange
            var cliente = new Cliente("Empresa Teste", "contato@teste.com") { Id = 1 };
            var dto = new ClienteDetalheDto { Id = 1, Nome = "Empresa Teste", Email = "contato@teste.com" };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(cliente);
            _mapperMock.Setup(m => m.Map<ClienteDetalheDto>(cliente)).Returns(dto);

            // Act
            var result = await _service.ObterPorIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Nome.Should().Be("Empresa Teste");

            _repositoryMock.Verify(r => r.ObterPorIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObterPorIdAsync_ComIdInvalido_DeveRetornarNull()
        {
            // Arrange
            _repositoryMock.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Cliente)null);

            // Act
            var result = await _service.ObterPorIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region Atualizar Cliente

        [Fact]
        public async Task AtualizarAsync_ComDadosValidos_DeveAtualizarClienteComSucesso()
        {
            // Arrange
            var id = 1;
            var clienteExistente = new Cliente("Nome Antigo", "email@teste.com") { Id = id };
            var dto = new ClienteDetalheDto { Id = id, Nome = "Nome Novo", Email = "novoemail@teste.com" };
            var clienteAtualizado = new Cliente(dto.Nome, dto.Email) { Id = id };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(clienteExistente);
            _mapperMock.Setup(m => m.Map<Cliente>(dto)).Returns(clienteAtualizado);
            _validatorMock.Setup(v => v.ValidateAsync(clienteAtualizado, default))
                .ReturnsAsync(new ValidationResult());
            _repositoryMock.Setup(r => r.AtualizarAsync(clienteAtualizado)).Returns(Task.CompletedTask);

            // Act
            await _service.AtualizarAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
            _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_ComClienteNaoEncontrado_DeveLancarExcecao()
        {
            // Arrange
            var dto = new ClienteDetalheDto { Id = 999, Nome = "Teste", Email = "teste@teste.com" };
            _repositoryMock.Setup(r => r.ObterPorIdAsync(999)).ReturnsAsync((Cliente)null);

            // Act
            Func<Task> act = async () => await _service.AtualizarAsync(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cliente não encontrado.");
            _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_SemLogotipo_DevePreservarLogotipoExistente()
        {
            // Arrange
            var id = 1;
            var logotipoExistente = new byte[] { 1, 2, 3 };
            var clienteExistente = new Cliente("Nome", "email@teste.com")
            {
                Id = id,
                Logotipo = logotipoExistente
            };
            var dto = new ClienteDetalheDto { Id = id, Nome = "Nome Novo", Email = "novo@teste.com", Logotipo = null };
            var clienteAtualizado = new Cliente(dto.Nome, dto.Email) { Id = id, Logotipo = logotipoExistente };

            _repositoryMock.Setup(r => r.ObterPorIdAsync(id)).ReturnsAsync(clienteExistente);
            _mapperMock.Setup(m => m.Map<Cliente>(dto)).Returns(clienteAtualizado);
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<Cliente>(), default))
                .ReturnsAsync(new ValidationResult());

            // Act
            await _service.AtualizarAsync(dto);

            // Assert
            _repositoryMock.Verify(r => r.AtualizarAsync(It.Is<Cliente>(c => c.Logotipo == logotipoExistente)), Times.Once);
        }

        #endregion

        #region Remover Cliente

        [Fact]
        public async Task RemoverAsync_ComIdValido_DeveRemoverClienteComSucesso()
        {
            // Arrange
            var id = 1;
            _repositoryMock.Setup(r => r.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            await _service.RemoverAsync(id);

            // Assert
            _repositoryMock.Verify(r => r.RemoverAsync(id), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_DeveExecutarSemErro()
        {
            // Arrange
            var id = 999;
            _repositoryMock.Setup(r => r.RemoverAsync(id)).Returns(Task.CompletedTask);

            // Act
            Func<Task> act = async () => await _service.RemoverAsync(id);

            // Assert
            await act.Should().NotThrowAsync();
        }

        #endregion
    }
}
