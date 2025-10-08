using BootstrapBlazorApp2.Server.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BootstrapBlazorApp2.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TipoPessoa> TiposPessoa { get; set; }
        public DbSet<SubtipoPessoa> SubtiposPessoa { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade TipoPessoa
            modelBuilder.Entity<TipoPessoa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(50);

                // Configuração do relacionamento um-para-muitos
                entity.HasMany(e => e.SubtiposPessoa)
                      .WithOne(e => e.TipoPessoa)
                      .HasForeignKey(e => e.TipoPessoaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração da entidade SubtipoPessoa
            modelBuilder.Entity<SubtipoPessoa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Descricao).IsRequired().HasMaxLength(100);
            });

            // Seeding de dados
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed TipoPessoa
            var tiposPessoa = new[]
            {
            new TipoPessoa { Id = 1, Descricao = "Física" },
            new TipoPessoa { Id = 2, Descricao = "Jurídica" },
            new TipoPessoa { Id = 3, Descricao = "Governamental" }
        };

            modelBuilder.Entity<TipoPessoa>().HasData(tiposPessoa);

            // Seed SubtipoPessoa
            var subtiposPessoa = new[]
            {
            // Subtipos para Pessoa Física (Id = 1)
            new SubtipoPessoa { Id = 1, Descricao = "Individual", TipoPessoaId = 1 },
            new SubtipoPessoa { Id = 2, Descricao = "Profissional Liberal", TipoPessoaId = 1 },
            
            // Subtipos para Pessoa Jurídica (Id = 2)
            new SubtipoPessoa { Id = 3, Descricao = "Empresa Privada", TipoPessoaId = 2 },
            new SubtipoPessoa { Id = 4, Descricao = "Organização Sem Fins Lucrativos", TipoPessoaId = 2 },
            
            // Subtipos para Pessoa Governamental (Id = 3)
            new SubtipoPessoa { Id = 5, Descricao = "Órgão Federal", TipoPessoaId = 3 },
            new SubtipoPessoa { Id = 6, Descricao = "Órgão Estadual", TipoPessoaId = 3 }
        };

            modelBuilder.Entity<SubtipoPessoa>().HasData(subtiposPessoa);
        }
    }
}
