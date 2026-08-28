using Finanzas.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Identity;

namespace Finanzas.Infrastructure.Identidad;


// Implementación de IServicioVerificacionEmail sobre UserManager<ApplicationUser>.
// Se usa solo para generar/confirmar el token: la creación del usuario sigue
// pasando por UsuarioRepository (EF directo), no por UserManager, para no
// romper la atomicidad del registro (ver AuthService.RegistrarAsync).

internal sealed class ServicioVerificacionEmail : IServicioVerificacionEmail
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ServicioVerificacionEmail(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> GenerarTokenAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe un usuario con Id {usuarioId} para generar el token.");

        return await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
    }

    public async Task<bool> ConfirmarAsync(Guid usuarioId, string token, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(usuarioId.ToString())
            ?? throw new InvalidOperationException($"No existe un usuario con Id {usuarioId} para confirmar el email.");

        var resultado = await _userManager.ConfirmEmailAsync(appUser, token);
        return resultado.Succeeded;
    }
}
