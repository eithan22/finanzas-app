using Finanzas.Domain.Entidades;

namespace Finanzas.Domain.Interfaces;


// Contrato de acceso a datos para Usuario. La implementación vive en
// Infrastructure (EF Core).

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default);
    void Actualizar(Usuario usuario);
    void Eliminar(Usuario usuario);
}
