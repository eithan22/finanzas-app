using Finanzas.Domain.Entidades;

namespace Finanzas.Domain.Interfaces;


// Contrato de acceso a datos para Usuario. La implementación vive en
// Infrastructure (EF Core) y es el único punto del sistema que conoce
// ASP.NET Identity: por acá solo entra y sale Usuario de dominio.

public interface IUsuarioRepository : IRepositorioBase<Usuario>
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
}
