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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.Run();