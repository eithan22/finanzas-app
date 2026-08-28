using Finanzas.Domain.Entidades;
using Finanzas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.Infrastructure.Persistencia.Repositorios;


// Acceso a datos de Categoría. Las escrituras vienen de RepositorioBase.

public class CategoriaRepository : RepositorioBase<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(FinanzasDbContext contexto) : base(contexto)
    {
    }

    // El usuarioId no es opcional: filtrar por él en la misma consulta es lo
    // que impide que un usuario lea una categoría de otro (RF-28). Si el Id
    // existe pero es de otro usuario, devuelve null igual que si no existiera.
    public Task<Categoria?> ObtenerPorIdAsync(Guid id, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return Contexto.Categorias
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId, cancellationToken);
    }

    public async Task<IReadOnlyList<Categoria>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await Contexto.Categorias
            .Where(c => c.UsuarioId == usuarioId)
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }
}
