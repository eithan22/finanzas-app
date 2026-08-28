using Finanzas.Domain.Common;

namespace Finanzas.Domain.Entidades;


// Refresco de sesión (RF-27). Vive aparte de AspNetUserTokens porque
// necesita expiración y revocación propias, no solo un provider/purpose de
// Identity.

public class RefreshToken : EntidadBase
{
    public Guid UsuarioId { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; }

    public DateTime FechaExpiracion { get; set; }

    public bool Revocado { get; set; }
}
