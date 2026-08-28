using Finanzas.Domain.Interfaces;

namespace Finanzas.Infrastructure.Persistencia;


// Implementación de IUnitOfWork sobre EF Core. Es una envoltura mínima del
// SaveChangesAsync del contexto: existe para que Application pueda confirmar
// los cambios sin conocer EF Core ni el DbContext.

public class UnitOfWork : IUnitOfWork
{
    private readonly FinanzasDbContext _contexto;

    public UnitOfWork(FinanzasDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default)
    {
        return _contexto.SaveChangesAsync(cancellationToken);
    }
}
