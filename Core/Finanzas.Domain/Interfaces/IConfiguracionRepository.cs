using Finanzas.Domain.Entidades;

namespace Finanzas.Domain.Interfaces;


// Contrato de acceso a datos para la Configuración del usuario (1:1).
//Implementación en Infrastructure (EF Core). RF-28: siempre por usuarioId.

public interface IConfiguracionRepository
{
    Task<Configuracion?> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task AgregarAsync(Configuracion configuracion, CancellationToken cancellationToken = default);
    void Actualizar(Configuracion configuracion);
}
