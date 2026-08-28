using Finanzas.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;

namespace Finanzas.Infrastructure.Identidad;

internal sealed class ServicioRecuperacionPassword : IServicioRecuperacionPassword
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ServicioRecuperacionPassword(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerarTokenAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe un usuario con Id {usuarioId} para generar el token de recuperación.");

        return await _userManager.GeneratePasswordResetTokenAsync(appUser);
    }

    // Único lugar donde se deja que UserManager escriba el PasswordHash
    // directo: ResetPasswordAsync regenera el SecurityStamp en la misma
    // operación, que es lo que invalida el token usado (RF-29, un solo uso).
    public async Task<bool> RestablecerAsync(Guid usuarioId, string token, string nuevoPasswordPlano, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe un usuario con Id {usuarioId} para restablecer la contraseña.");

        var resultado = await _userManager.ResetPasswordAsync(appUser, token, nuevoPasswordPlano);
        return resultado.Succeeded;
    }
}
