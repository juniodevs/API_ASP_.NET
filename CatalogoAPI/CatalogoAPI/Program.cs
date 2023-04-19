using CatalogoAPI.Context;
using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); //
builder.Services.AddSwaggerGen(); // Swagger

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => 
                                            options
                                            .UseMySql(connectionString,
                                            ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

//definir endpoints

app.MapGet("/", () => "Catálogo de Produtos - 2023");

app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

app.MapGet("/categorias", async(AppDbContext db) => await db.Categorias.ToListAsync());

app.MapGet("/categorias/{id:int}", async(int id, AppDbContext db) =>
{
    return await db.Categorias.FindAsync(id)
    is Categoria categoria
    ? Results.Ok(categoria)
    : Results.NotFound();
});

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
}
);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.Run();