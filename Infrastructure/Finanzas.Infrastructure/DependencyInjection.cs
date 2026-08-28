using Finanzas.Domain.Interfaces;
using Finanzas.Infrastructure.Identidad;
using Finanzas.Infrastructure.Persistencia;
using Finanzas.Infrastructure.Persistencia.Repositorios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
