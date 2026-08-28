using Finanzas.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Base de datos, Identity y repositorios (ver Finanzas.Infrastructure).
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("FinanzasDb")
        ?? throw new InvalidOperationException(
            "Falta la cadena de conexión 'FinanzasDb' en la configuración."));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
