using FluentValidation;
using SistemaGestao.Domain.Entities;

namespace SistemaGestao.Domain.Validations
{
    public class ClienteValidator : AbstractValidator<Cliente>
    {
        public ClienteValidator()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("E-mail inválido.");

            RuleFor(c => c.Logotipo)
                .Must(b => b == null || b.Length > 0)
                .WithMessage("O arquivo de logotipo não pode estar vazio.");
        }
    }
}