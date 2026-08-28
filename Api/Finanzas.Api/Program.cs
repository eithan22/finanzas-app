using Finanzas.Api.Servicios;
using Finanzas.Application;
using Finanzas.Application.Interfaces.IServices;
using Finanzas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Servicios de negocio, validadores y mappers (ver Finanzas.Application).
builder.Services.AddApplication();

// Lee el UsuarioId del JWT autenticado (RF-28).
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActualService, UsuarioActualService>();

// Base de datos, Identity y repositorios (ver Finanzas.Infrastructure).
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("FinanzasDb")
        ?? throw new InvalidOperationException(
            "Falta la cadena de conexión 'FinanzasDb' en la configuración."));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
