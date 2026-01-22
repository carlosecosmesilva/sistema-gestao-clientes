using Dapper;
using Microsoft.EntityFrameworkCore;
using SistemaGestao.Domain.Entities;
using SistemaGestao.Domain.Interfaces;
using SistemaGestao.Infra.Data.Context;
using System.Data;
using System.Linq;

namespace SistemaGestao.Infra.Data.Repositories
{
    public class ClienteRepository(ApplicationDbContext context) : IClienteRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Cliente>> ObterTodosAsync()
        {
            // Não está sendo enviado o campo 'Logotipo' (Blob) na listagem geral.
            // Com isso é economizado memória e tempo de transferência de dados.
            return await _context.Clientes
                .AsNoTracking()
                .Select(c => new Cliente
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Email = c.Email,
                    Telefone = c.Telefone,
                    // Logotipo é ignorado propositalmente aqui
                    Logradouros = c.Logradouros // Endereços são trazidos se necessário
                })
                .ToListAsync();
        }

        public async Task<Cliente?> ObterPorIdAsync(int id)
        {
            return await _context.Clientes
                .Include(c => c.Logradouros)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Clientes
                .AnyAsync(c => c.Email == email);
        }

        public async Task<Cliente> AdicionarAsync(Cliente cliente)
        {
            var connection = _context.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Nome", cliente.Nome);
            parameters.Add("@Email", cliente.Email);
            parameters.Add("@Telefone", cliente.Telefone);
            parameters.Add("@Logotipo", cliente.Logotipo, DbType.Binary);

            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_AdicionarCliente",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            cliente.Id = parameters.Get<int>("@Id");

            // Adicionar logradouros se existirem
            if (cliente.Logradouros != null && cliente.Logradouros.Any())
            {
                foreach (var logradouro in cliente.Logradouros)
                {
                    logradouro.ClienteId = cliente.Id;
                    var logParams = new DynamicParameters();
                    logParams.Add("@ClienteId", cliente.Id);
                    logParams.Add("@Endereco", logradouro.Endereco);
                    logParams.Add("@Complemento", logradouro.Complemento);
                    logParams.Add("@Bairro", logradouro.Bairro);
                    logParams.Add("@Cidade", logradouro.Cidade);
                    logParams.Add("@Estado", logradouro.Estado);
                    logParams.Add("@CEP", logradouro.CEP);

                    await connection.ExecuteAsync(
                        "sp_AdicionarLogradouro",
                        logParams,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }

            return cliente;
        }

        public async Task AtualizarAsync(Cliente cliente)
        {
            var connection = _context.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", cliente.Id);
            parameters.Add("@Nome", cliente.Nome);
            parameters.Add("@Email", cliente.Email);
            parameters.Add("@Telefone", cliente.Telefone);
            parameters.Add("@Logotipo", cliente.Logotipo, DbType.Binary);

            await connection.ExecuteAsync(
                "sp_AtualizarCliente",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Atualizar logradouros
            if (cliente.Logradouros != null)
            {
                // Remover logradouros existentes
                await connection.ExecuteAsync(
                    "DELETE FROM Logradouros WHERE ClienteId = @ClienteId",
                    new { ClienteId = cliente.Id }
                );

                // Adicionar novos logradouros
                foreach (var logradouro in cliente.Logradouros)
                {
                    logradouro.ClienteId = cliente.Id;
                    var logParams = new DynamicParameters();
                    logParams.Add("@ClienteId", cliente.Id);
                    logParams.Add("@Endereco", logradouro.Endereco);
                    logParams.Add("@Complemento", logradouro.Complemento);
                    logParams.Add("@Bairro", logradouro.Bairro);
                    logParams.Add("@Cidade", logradouro.Cidade);
                    logParams.Add("@Estado", logradouro.Estado);
                    logParams.Add("@CEP", logradouro.CEP);

                    await connection.ExecuteAsync(
                        "sp_AdicionarLogradouro",
                        logParams,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
        }

        public async Task RemoverAsync(int id)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_ExcluirCliente",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}