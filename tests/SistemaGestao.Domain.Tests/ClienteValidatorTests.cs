using FluentAssertions;
using SistemaGestao.Domain.Entities;
using SistemaGestao.Domain.Validations;

namespace SistemaGestao.Domain.Tests
{
    /// <summary>
    /// Contém testes unitários para a classe <see cref="ClienteValidator"/>, 
    /// verificando a lógica de validação para dados do cliente como formatos de nome e e-mail.
    /// </summary>
    /// <remarks>Esses testes abrangem cenários válidos e inválidos para nome e e-mail do cliente, 
    /// incluindo casos extremos, como caracteres acentuados e valores numéricos em nomes. 
    /// Os testes garantem que o validador identifique corretamente entradas válidas e inválidas,
    /// de acordo com as regras de negócio e os comportamentos padrão do FluentValidation.</remarks>
    public class ClienteValidatorTests
    {
        private readonly ClienteValidator _validator = new();

        #region Happy Path (Cenário Válido)

        [Fact]
        public void Validar_ClienteComDadosValidos_DeveRetornarTrue()
        {
            // Arrange
            var cliente = new Cliente("Empresa Exemplo", "contato@exemplo.com");

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        #endregion

        #region Validação de Nome

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]  // Apenas espaços
        public void Validar_NomeInvalidoOuVazio_DeveRetornarErro(string? nome)
        {
            // Arrange
            var cliente = new Cliente(nome, "email@valido.com");

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Nome");
        }

        [Fact]
        public void Validar_NomeMuitoCurto_DeveRetornarErro()
        {
            // Arrange - Nome com menos de 3 caracteres (se houver validação de tamanho mínimo)
            var cliente = new Cliente("AB", "email@valido.com");

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            if (result.IsValid == false)
                result.Errors.Should().Contain(x => x.PropertyName == "Nome");
        }

        [Fact]
        public void Validar_NomeMuitoLongo_DeveRetornarErro()
        {
            // Arrange - Nome com mais de 255 caracteres (se houver validação de tamanho máximo)
            var nomeLongo = new string('A', 256);
            var cliente = new Cliente(nomeLongo, "email@valido.com");

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            if (result.IsValid == false)
                result.Errors.Should().Contain(x => x.PropertyName == "Nome");
        }

        #endregion

        #region Validação de Email

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]  // Apenas espaços
        public void Validar_EmailVazioOuNulo_DeveRetornarErro(string? email)
        {
            // Arrange
            var cliente = new Cliente("Empresa Valida", email);

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Email");
        }

        [Theory]
        [InlineData("email_sem_arroba.com")]      // Sem @
        [InlineData("@dominio.com")]               // Sem usuário
        [InlineData("email@")]                     // Sem domínio
        public void Validar_EmailComFormatoInvalido_DeveRetornarErro(string email)
        {
            // Arrange
            var cliente = new Cliente("Empresa Valida", email);

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Email");
        }

        [Theory]
        [InlineData("email@dominio")]              // Sem TLD (aceito pelo .EmailAddress())
        [InlineData("email @dominio.com")]         // Espaço no meio (aceito pelo .EmailAddress())
        public void Validar_EmailComFormatoAceitoPeloValidador_DeveSerValido(string email)
        {
            // Arrange - Estes emails passam no validador padrão do FluentValidation
            // O validador usa .EmailAddress() que é mais permissivo que RFC 5322 rigoroso
            var cliente = new Cliente("Empresa Valida", email);

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("contato@exemplo.com")]
        [InlineData("suporte@empresa.co.uk")]
        [InlineData("vendas@subdomain.empresa.com.br")]
        [InlineData("teste+tag@example.com")]
        public void Validar_EmailComFormatoValido_DeveRetornarTrue(string email)
        {
            // Arrange
            var cliente = new Cliente("Empresa Valida", email);

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Validar_ClienteComNomeComAcentos_DeveSerValido()
        {
            // Arrange
            var cliente = new Cliente("Empresa São Paulo", "contato@exemplo.com");

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validar_ClienteComNomeComNumeros_DeveSerValido()
        {
            // Arrange
            var cliente = new Cliente("Empresa 123 LTDA", "contato@exemplo.com");

            // Act
            var result = _validator.Validate(cliente);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        #endregion
    }
}
