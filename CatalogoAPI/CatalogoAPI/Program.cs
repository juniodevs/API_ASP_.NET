using CatalogoAPI.Context;
using CatalogoAPI.Models;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); //
builder.Services.AddSwaggerGen(); // Swagger

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => 
                                            options
                                            .UseMySql(connectionString,
                                            ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<ITokenServices>(new TokenService());

builder.Services.AddAuthentication
        (JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
               options.TokenValidationParameters = new TokenValidationParameters
        {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

builder.Services.AddAuthorization();

var app = builder.Build();

// endpoint para LOGIN

app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenServices tokenService) =>
{
    if (userModel == null)
    {
        return Results.BadRequest("Login Inválido");
    }
    if(userModel.Username == "junio" && (userModel.Password == "123@123"))
    {
            var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
                                    app.Configuration["Jwt:Issuer"],
                                    app.Configuration["Jwt:Audience"],
                                    userModel);

        return Results.Ok(new { token = tokenString });
    }
    else
        return Results.BadRequest("Login Inválido");
}).Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status200OK)
.WithName("Login")
.WithTags("Authenticacao");

//definir endpoints para CATEGORIAS
app.MapGet("/", () => "Catálogo de Produtos - 2023").ExcludeFromDescription().RequireAuthorization();

app.MapPost("/categorias", async (Categoria categoria, AppDbContext db) =>
{
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();

    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

app.MapGet("/categorias", async(AppDbContext db) => await db.Categorias.ToListAsync()).RequireAuthorization();

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
});

app.MapDelete("/categorias/{id:int}", async(int id, AppDbContext db) =>
{

    var categoria = await db.Categorias.FindAsync(id);
    if (categoria is null) return Results.NotFound("Categoria já é vazia");

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


//definir endpoints para PRODUTOS

app.MapPost("/produtos", async(AppDbContext db, Produto produto) =>
{
    if (produto is null)
    {
        return Results.NoContent();
    }
    db.Produtos.Add(produto);
    await db.SaveChangesAsync();

    return Results.Created($"/produtos/{produto.CategoriaId}", produto);
});

app.MapGet("/produtos", async(AppDbContext db) => await db.Produtos.ToListAsync()).RequireAuthorization();

app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Produtos.FindAsync(id)
    is Produto produto
    ? Results.Ok(produto)
    : Results.NotFound();
});

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
});

app.MapDelete("produtos/{id:int}", async (int id, AppDbContext db) =>
{
    var produto = await db.Produtos.FindAsync(id);

    if(produto is null)
    {
        return Results.NoContent();
    }
    db.Produtos.Remove(produto);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.Run();