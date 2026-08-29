using Finanzas.Api.Middleware;
using Finanzas.Api.Servicios;
using Finanzas.Application;
using Finanzas.Application.Interfaces.IServices;
using Finanzas.Infrastructure;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging estructurado: la configuración completa (niveles, sinks) vive en appsettings.
builder.Services.AddSerilog((services, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Traduce cualquier excepción no atrapada a una respuesta ProblemDetails.
builder.Services.AddExceptionHandler<ManejadorExcepciones>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá el token de acceso (sin el prefijo 'Bearer ')."
    });

    opciones.AddSecurityRequirement(document => new()
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Loguea cada request (método, ruta, status, duración) sin código manual por endpoint.
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
