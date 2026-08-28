using Finanzas.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;

namespace Finanzas.Infrastructure.Identidad;


// Implementación de IServicioHashPassword sobre el hasher de ASP.NET Identity.
// Usamos el de Identity y no uno propio porque ya resuelve bien las partes
// difíciles: el algoritmo, la cantidad de repeticiones y la sal aleatoria por
// usuario. El hashing de contraseñas es de esas cosas que no conviene escribir
// a mano.

internal sealed class ServicioHashPassword : IServicioHashPassword
{
    private readonly IPasswordHasher<ApplicationUser> _hasher;

    public ServicioHashPassword(IPasswordHasher<ApplicationUser> hasher)
    {
        _hasher = hasher;
    }

    public string Hashear(string passwordPlano)
    {
        // El primer parámetro es un usuario, pero el hasher de Identity no lo
        // mira: la sal la genera al azar y la guarda dentro del propio hash.
        // Por eso se le pasa un objeto vacío y no el usuario real.
        return _hasher.HashPassword(new ApplicationUser(), passwordPlano);
    }

    public bool Verificar(string passwordPlano, string hashGuardado)
    {
        var resultado = _hasher.VerifyHashedPassword(
            new ApplicationUser(), hashGuardado, passwordPlano);

        return resultado != PasswordVerificationResult.Failed;
    }
}
