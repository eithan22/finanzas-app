using Finanzas.Domain.Entidades;

namespace Finanzas.Domain.Interfaces;

// Contrato de acceso a datos para Categoría. La implementación vive en
// Infrastructure (EF Core). Todas las operaciones que reciben usuarioId
// deben filtrar por él para garantizar el aislamiento de datos (RF-28).

public interface ICategoriaRepository : IRepositorioBase<Categoria>
{
    Task<Categoria?> ObtenerPorIdAsync(Guid id, Guid usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Categoria>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    // Alta en lote (ej. categorías por defecto del registro, RF-26): una
    // sola llamada a AddRangeAsync en vez de varios AgregarAsync sueltos,
    // que no se pueden paralelizar porque el DbContext no es thread-safe.
    Task AgregarVariasAsync(IEnumerable<Categoria> categorias, CancellationToken cancellationToken = default);
}
