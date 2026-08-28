using Finanzas.Domain.Entidades;
using Finanzas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.Infrastructure.Persistencia.Repositorios;

public class RefreshTokenRepository : RepositorioBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(FinanzasDbContext contexto) : base(contexto)
    {
    }

    public Task<RefreshToken?> ObtenerPorTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return Contexto.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);
    }
}
