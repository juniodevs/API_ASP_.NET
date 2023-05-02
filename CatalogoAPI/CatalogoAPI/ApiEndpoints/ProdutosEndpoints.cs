using CatalogoAPI.Context;
using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.ApiEndpoints
{
    public static class ProdutosEndpoints
    {
        public static void MapProdutosEndpoints(this WebApplication app)
        {
            app.MapPost("/produtos", async (AppDbContext db, Produto produto) =>
            {
                if (produto is null)
                {
                    return Results.NoContent();
                }
                db.Produtos.Add(produto);
                await db.SaveChangesAsync();

                return Results.Created($"/produtos/{produto.CategoriaId}", produto);
            }).WithTags("Produtos");

            app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos
            .ToListAsync())
            .WithTags("Produtos")
            .RequireAuthorization();

            app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
            {
                return await db.Produtos.FindAsync(id)
                is Produto produto
                ? Results.Ok(produto)
                : Results.NotFound();
            }).WithTags("Produtos");

            app.MapPut("/produtos/{id:int}", async (int id, AppDbContext db, Produto produto) =>
            {
                if (produto.ProdutoId != id)
                {
                    Results.BadRequest();
                }

                var produtoDB = await db.Produtos.FindAsync(id);

                produtoDB.Nome = produto.Nome;
                produtoDB.Imagem = produto.Imagem;
                produtoDB.Descricao = produto.Descricao;
                produtoDB.Preco = produto.Preco;
                produtoDB.DataCompra = produto.DataCompra;
                produtoDB.Estoque = produto.Estoque;
                produtoDB.CategoriaId = produto.CategoriaId;

                await db.SaveChangesAsync();

                return Results.Ok(produtoDB);
            }).WithTags("Produtos");

            app.MapDelete("produtos/{id:int}", async (int id, AppDbContext db) =>
            {
                var produto = await db.Produtos.FindAsync(id);

                if (produto is null)
                {
                    return Results.NoContent();
                }
                db.Produtos.Remove(produto);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }).WithTags("Produtos");
        }
    }
}
