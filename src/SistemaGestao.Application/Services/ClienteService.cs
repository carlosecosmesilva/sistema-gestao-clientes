using AutoMapper;
using FluentValidation;
using SistemaGestao.Application.DTOs;
using SistemaGestao.Domain.Entities;
using SistemaGestao.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaGestao.Application.Services
{
    public class ClienteService(IClienteRepository repository, IMapper mapper, IValidator<Cliente> validator) : IClienteService
    {
        private readonly IClienteRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<Cliente> _validator = validator;

        public async Task<IEnumerable<ClienteResumoDto>> ObterTodosAsync()
        {
            var clientes = await _repository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<ClienteResumoDto>>(clientes);
        }

        public async Task<ClienteDetalheDto> ObterPorIdAsync(int id)
        {
            var cliente = await _repository.ObterPorIdAsync(id);
            return _mapper.Map<ClienteDetalheDto>(cliente);
        }

        public async Task<ClienteResultDto> CriarAsync(ClienteCreateDto dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);

            // 2. Valida Regras de Domínio (FluentValidation)
            var validationResult = await _validator.ValidateAsync(cliente);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // 3. Valida Regra de Negócio Específica (Unicidade de Email)
            if (await _repository.ExisteEmailAsync(cliente.Email))
                throw new Exception("Este e-mail já está cadastrado.");

            // 4. Persiste (Chama a Stored Procedure via Repo)
            var clienteCriado = await _repository.AdicionarAsync(cliente);

            return new ClienteResultDto { Id = clienteCriado.Id, Nome = clienteCriado.Nome };
        }

        public async Task AtualizarAsync(ClienteDetalheDto dto)
        {
            var cliente = _mapper.Map<Cliente>(dto);

            // Validação de Regras de Domínio
            var validationResult = await _validator.ValidateAsync(cliente);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await _repository.AtualizarAsync(cliente);
        }

        public async Task RemoverAsync(int id)
        {
            await _repository.RemoverAsync(id);
        }
    }
}