using Finanzas.Domain.Entidades;

namespace Finanzas.Domain.Interfaces;


// Contrato de acceso a datos para RefreshToken (RF-27).

public interface IRefreshTokenRepository : IRepositorioBase<RefreshToken>
{
    Task<RefreshToken?> ObtenerPorTokenAsync(string token, CancellationToken cancellationToken = default);
}
