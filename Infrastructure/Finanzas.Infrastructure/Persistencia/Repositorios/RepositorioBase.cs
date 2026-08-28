using Finanzas.Domain.Interfaces;

namespace Finanzas.Infrastructure.Persistencia.Repositorios;


// Implementación común de las operaciones de escritura de IRepositorioBase.
// Los tres métodos solo MARCAN el cambio en el contexto: nada llega a la base
// hasta que alguien llama a IUnitOfWork.GuardarCambiosAsync.

public abstract class RepositorioBase<TEntidad> : IRepositorioBase<TEntidad>
    where TEntidad : class
{
    protected readonly FinanzasDbContext Contexto;

    protected RepositorioBase(FinanzasDbContext contexto)
    {
        Contexto = contexto;
    }

    public async Task AgregarAsync(TEntidad entidad, CancellationToken cancellationToken = default)
    {
        await Contexto.Set<TEntidad>().AddAsync(entidad, cancellationToken);
    }

    public void Actualizar(TEntidad entidad)
    {
        Contexto.Set<TEntidad>().Update(entidad);
    }

    public void Eliminar(TEntidad entidad)
    {
        Contexto.Set<TEntidad>().Remove(entidad);
    }
}
