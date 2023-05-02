using CatalogoAPI.ApiEndpoints;
using CatalogoAPI.AppServicesExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiSwagger();
builder.AdddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJwt();

var app = builder.Build();

app.MapAuthenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();

var environment = app.Environment;
app.useExceptionHandling(environment)
    .UseSwaggerMiddleware()
    .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();