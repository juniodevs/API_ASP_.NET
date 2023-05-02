using CatalogoAPI.Context;
using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.ApiEndpoints
{
    public static class CategoriasEndpoints
    {
        public static void MapCategoriasEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => "Catálogo de Produtos - 2023").ExcludeFromDescription();

            app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
            {
                db.Categorias.Add(categoria);
                await db.SaveChangesAsync();

                return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
            }).WithTags("Categorias");

            app.MapGet("/categorias", async (AppDbContext db) => await db.Categorias
            .ToListAsync())
            .WithTags("Categorias")
            .RequireAuthorization();

            app.MapGet("/categorias/{id:int}", async (int id, AppDbContext db) =>
            {
                return await db.Categorias.FindAsync(id)
                is Categoria categoria
                ? Results.Ok(categoria)
                : Results.NotFound();
            }).WithTags("Categorias");

            app.MapPut("/categorias/{id:int}", async (int id, Categoria categoria, AppDbContext db) =>
            {
                if (categoria.CategoriaId != id)
                {
                    return Results.BadRequest("Id da Categoria diferente do ID");
                }

                var categoriaDB = await db.Categorias.FindAsync(id);

                if (categoriaDB is null) return Results.NotFound();

                categoriaDB.Nome = categoria.Nome;
                categoriaDB.Descricao = categoria.Descricao;

                await db.SaveChangesAsync();
                return Results.Ok(categoriaDB);
            }).WithTags("Categorias");

            app.MapDelete("/categorias/{id:int}", async (int id, AppDbContext db) =>
            {

                var categoria = await db.Categorias.FindAsync(id);
                if (categoria is null) return Results.NotFound("Categoria já é vazia");

                db.Categorias.Remove(categoria);
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).WithTags("Categorias");
        }

    }
}
