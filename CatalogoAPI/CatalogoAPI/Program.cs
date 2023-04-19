using CatalogoAPI.Context;
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

app.MapPost("/categorias",);


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.Run();