using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            //Categoria
            mb.Entity<Categoria>().HasKey(c => c.CategoriaId);
            mb.Entity<Categoria>().Property(c => c.Nome).IsRequired().HasMaxLength(100);

            mb.Entity<Categoria>().Property(c => c.Descricao)
                                  .IsRequired()
                                  .HasMaxLength(150);

            //Produto
            mb.Entity<Produto>().HasKey(p => p.ProdutoId);
            mb.Entity<Produto>().Property(p => p.Nome).IsRequired().HasMaxLength(100);
            mb.Entity<Produto>().Property(p => p.Descricao).IsRequired().HasMaxLength(150);
            mb.Entity<Produto>().Property(p => p.Imagem).IsRequired().HasMaxLength(100);
            mb.Entity<Produto>().Property(p => p.Preco).HasPrecision(14, 2);

            //Relacionamento

            mb.Entity<Produto>().HasOne(p => p.Categoria)
                                .WithMany(c => c.Produtos)
                                .HasForeignKey(p => p.CategoriaId);
        }
    }
}
