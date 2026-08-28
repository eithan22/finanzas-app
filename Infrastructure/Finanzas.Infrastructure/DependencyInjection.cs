using System.Text;
using Finanzas.Application.Interfaces.IServices;
using Finanzas.Domain.Interfaces;
using Finanzas.Infrastructure.Identidad;
using Finanzas.Infrastructure.Persistencia;
using Finanzas.Infrastructure.Persistencia.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Finanzas.Infrastructure;


// Registro de todo lo que aporta Infrastructure. Está acá y no suelto en
// Program.cs para que la API no tenga que saber qué motor de base de datos ni
// qué sistema de identidad hay detrás: solo llama a AddInfrastructure.

// Recibe la cadena de conexión como string y no IConfiguration para no atar
// este proyecto a cómo la API guarda su configuración.

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string cadenaConexion)
    {
        services.AddDbContext<FinanzasDbContext>(opciones =>
            opciones.UseSqlServer(cadenaConexion));

        // Los tokens de verificación de email (RF-25) y de recuperación de
        // contraseña (RF-29) que genera Identity van firmados y cifrados con
        // Data Protection, que no viene registrado por defecto. Se registra
        // acá, junto a lo que lo necesita.
        // Pendiente para Despliegue: por defecto las claves se guardan en el
        // disco local de la máquina, así que un redeploy o una segunda
        // instancia invalidarían los tokens ya emitidos.
        services.AddDataProtection();

        // AddIdentityCore y no AddIdentity: la autenticación va por JWT
        // (RF-27), así que no queremos el esquema de cookies que AddIdentity
        // registra por defecto.
        services.AddIdentityCore<ApplicationUser>(opciones =>
            {
                // RF-30: política de contraseñas.
                opciones.Password.RequiredLength = 8;
                opciones.Password.RequireUppercase = true;
                opciones.Password.RequireLowercase = true;
                opciones.Password.RequireDigit = true;
                opciones.Password.RequireNonAlphanumeric = true;

                // RF-24 y RF-25: un email por cuenta, y verificado para entrar.
                opciones.User.RequireUniqueEmail = true;
                opciones.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<FinanzasDbContext>()
            // RF-25 y RF-29: tokens de verificación de email y de
            // recuperación de contraseña, de un solo uso y con expiración.
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Puente de hashing: Application pide IServicioHashPassword sin saber
        // que detrás está el hasher de Identity (RNF-02).
        services.AddScoped<IServicioHashPassword, ServicioHashPassword>();

        // Puente de verificación de email: Application pide un token sin
        // saber que detrás está UserManager (RF-26).
        services.AddScoped<IServicioVerificacionEmail, ServicioVerificacionEmail>();

        // Puente de JWT: Application pide un token de acceso/refresh sin
        // saber cómo se firma (RF-27).
        services.AddScoped<IServicioJwt, ServicioJwt>();

        // Puente de recuperación de contraseña: Application pide generar y
        // canjear un token sin saber que detrás está UserManager (RF-29).
        services.AddScoped<IServicioRecuperacionPassword, ServicioRecuperacionPassword>();

        // Validación del JWT en cada request autenticado. Los parámetros se
        // leen de IConfiguration recién cuando se resuelven las opciones
        // (no al llamar AddInfrastructure), así esta firma sigue sin
        // depender de IConfiguration.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IConfiguration>((opciones, configuracion) =>
            {
                opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuracion["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuracion["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracion["Jwt:Key"]!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
