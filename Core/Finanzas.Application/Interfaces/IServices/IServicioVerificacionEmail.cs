namespace Finanzas.Application.Interfaces.IServices;


// Genera y valida el token de un solo uso que confirma el email de un
// usuario (RF-26). La implementación vive en Infrastructure porque usa los
// token providers de Identity (ya registrados en AddInfrastructure);
// Application solo conoce el usuarioId y el token como string.

public interface IServicioVerificacionEmail
{
    Task<string> GenerarTokenAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<bool> ConfirmarAsync(Guid usuarioId, string token, CancellationToken cancellationToken = default);
}
