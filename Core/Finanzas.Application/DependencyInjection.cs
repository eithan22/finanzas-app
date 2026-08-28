using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Application.Interfaces.IServices;
using Finanzas.Application.Mappers;
using Finanzas.Application.Servicios;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Finanzas.Application;


// Registro de todo lo que aporta Application. Igual criterio que
// Infrastructure.DependencyInjection: la Api solo llama a AddApplication,
// sin saber qué hay adentro.

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IMapperUsuario, MapperUsuario>();
        services.AddScoped<IMapperCategoria, MapperCategoria>();

        // Registra todos los validadores de FluentValidation del ensamblado
        // (hoy solo RegistrarUsuarioDtoValidador; los que se agreguen después
        // quedan de alta solos, sin tocar este archivo).
        services.AddValidatorsFromAssemblyContaining<RegistrarUsuarioDtoValidador>();

        return services;
    }
}
