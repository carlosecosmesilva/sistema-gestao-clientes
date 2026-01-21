using Dapper;
using Microsoft.EntityFrameworkCore;
using SistemaGestao.Domain.Entities;
using SistemaGestao.Domain.Interfaces;
using SistemaGestao.Infra.Data.Context;
using System.Data;

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
            parameters.Add("@Logotipo", cliente.Logotipo, DbType.Binary);

            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sp_AdicionarCliente",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            cliente.Id = parameters.Get<int>("@Id");
            return cliente;
        }

        public async Task AtualizarAsync(Cliente cliente)
        {
            var connection = _context.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", cliente.Id);
            parameters.Add("@Nome", cliente.Nome);
            parameters.Add("@Email", cliente.Email);
            parameters.Add("@Logotipo", cliente.Logotipo, DbType.Binary);

            await connection.ExecuteAsync(
                "sp_AtualizarCliente",
                parameters,
                commandType: CommandType.StoredProcedure
            );
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