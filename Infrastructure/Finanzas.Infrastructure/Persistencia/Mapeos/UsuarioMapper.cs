using Finanzas.Domain.Entidades;
using Finanzas.Infrastructure.Identidad;
using Microsoft.AspNetCore.Identity;

namespace Finanzas.Infrastructure.Persistencia.Mapeos;


// Traducción manual entre la entidad de dominio Usuario y ApplicationUser
// (Identity). Es el mismo criterio que un mapper de DTO, pero en el borde de
// la persistencia en vez del borde de la API.

// internal a propósito: nadie fuera de Infrastructure debería poder usarlo.

internal static class UsuarioMapper
{
    // Identity -> Dominio.
    // Categorias y Configuracion no se llenan acá: se piden a sus propios
    // repositorios cuando un caso de uso las necesita.
    public static Usuario ADominio(ApplicationUser origen) => new()
    {
        Id = origen.Id,
        Email = origen.Email ?? string.Empty,
        PasswordHash = origen.PasswordHash ?? string.Empty,
        EmailVerificado = origen.EmailConfirmed,
        FechaCreacion = origen.FechaCreacion
    };

    // Dominio -> Identity, para altas.
    // Además de los campos del SRS hay que completar los que Identity da por
    // sentados: UserName y las versiones normalizadas (que son por las que
    // busca internamente), y el SecurityStamp, que es lo que permite
    // invalidar sesiones y tokens viejos al cambiar la contraseña.
    public static ApplicationUser ANuevoIdentity(Usuario origen, ILookupNormalizer normalizador) => new()
    {
        Id = origen.Id,
        Email = origen.Email,
        NormalizedEmail = normalizador.NormalizeEmail(origen.Email),
        UserName = origen.Email,
        NormalizedUserName = normalizador.NormalizeName(origen.Email),
        PasswordHash = origen.PasswordHash,
        EmailConfirmed = origen.EmailVerificado,
        FechaCreacion = origen.FechaCreacion,
        SecurityStamp = Guid.NewGuid().ToString()
    };

    // Dominio -> Identity, para ediciones: vuelca sobre la fila ya cargada
    // solo los campos que el dominio puede cambiar. FechaCreacion no está
    // porque no se modifica nunca después del alta.
    public static void VolcarCambios(Usuario origen, ApplicationUser destino, ILookupNormalizer normalizador)
    {
        destino.Email = origen.Email;
        destino.NormalizedEmail = normalizador.NormalizeEmail(origen.Email);
        destino.UserName = origen.Email;
        destino.NormalizedUserName = normalizador.NormalizeName(origen.Email);
        destino.PasswordHash = origen.PasswordHash;
        destino.EmailConfirmed = origen.EmailVerificado;
    }
}
