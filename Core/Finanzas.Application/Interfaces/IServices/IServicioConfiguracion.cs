using Finanzas.Application.Dtos;

namespace Finanzas.Application.Interfaces.IServices;

public interface IServicioConfiguracion
{
    Task<ConfiguracionResponseDto> ObtenerAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<ConfiguracionResponseDto> ActualizarAsync(Guid usuarioId, ActualizarConfiguracionDto dto, CancellationToken cancellationToken = default);
}
