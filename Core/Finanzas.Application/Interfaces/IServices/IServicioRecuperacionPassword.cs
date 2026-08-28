namespace Finanzas.Application.Interfaces.IServices;

public interface IServicioRecuperacionPassword
{
    Task<string> GenerarTokenAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<bool> RestablecerAsync(Guid usuarioId, string token, string nuevoPasswordPlano, CancellationToken cancellationToken = default);
}
