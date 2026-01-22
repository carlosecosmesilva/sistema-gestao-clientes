using Microsoft.EntityFrameworkCore;
using SistemaGestao.Domain.Entities;

namespace SistemaGestao.Infra.Data.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Logradouro> Logradouros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Configuração de Cliente ---
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // Utilização de Índice Único para garantir a regra de negócio no banco
                entity.HasIndex(e => e.Email).IsUnique();

                // Mapeamento do BLOB (Imagem)
                entity.Property(e => e.Logotipo)
                    .HasColumnType("varbinary(max)");
            });

            // --- Configuração de Logradouro ---
            modelBuilder.Entity<Logradouro>(entity =>
            {
                entity.ToTable("Logradouros");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Endereco).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Bairro).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Cidade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(2); // UF
                entity.Property(e => e.CEP).IsRequired().HasMaxLength(10);

                // Relacionamento: Um Logradouro tem um Cliente
                // DeleteBehavior.Cascade: Se apagar o Cliente, apaga os endereços
                entity.HasOne(d => d.Cliente)
                      .WithMany(p => p.Logradouros)
                      .HasForeignKey(d => d.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}