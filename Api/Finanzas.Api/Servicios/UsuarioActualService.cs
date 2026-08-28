using System.Security.Claims;
using Finanzas.Application.Interfaces.IServices;

namespace Finanzas.Api.Servicios;


// Lee el UsuarioId del claim del JWT ya autenticado (RF-28). Vive en Api y
// no en Infrastructure porque depende de IHttpContextAccessor, que es un
// concepto del pipeline HTTP — no algo que Infrastructure deba conocer.

public class UsuarioActualService : IUsuarioActualService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioActualService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UsuarioId
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("No hay un usuario autenticado en el request actual.");

            return Guid.Parse(claim);
        }
    }
}
