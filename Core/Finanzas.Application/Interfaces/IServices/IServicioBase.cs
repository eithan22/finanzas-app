namespace Finanzas.Application.Interfaces.IServices;


// Operaciones CRUD comunes a los servicios de entidades que pertenecen a un
// usuario. Cada interfaz de servicio concreta hereda de esta y agrega solo
// lo que tenga de propio.

// Los tres tipos genéricos son los tres DTOs que entran y salen: qué recibo
// al crear, qué recibo al actualizar, y qué devuelvo.

// El usuarioId va en TODOS los métodos y no es opcional: es lo que garantiza
// el aislamiento de datos (RF-28) por diseño y no por memoria del programador.
// Sale siempre del token del usuario autenticado, nunca del cuerpo del request.

public interface IServicioBase<TResponseDto, TCrearDto, TActualizarDto>
{
    Task<TResponseDto> CrearAsync(Guid usuarioId, TCrearDto dto, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResponseDto>> ObtenerTodosAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task<TResponseDto> ObtenerPorIdAsync(Guid usuarioId, Guid id, CancellationToken cancellationToken = default);

    Task<TResponseDto> ActualizarAsync(Guid usuarioId, Guid id, TActualizarDto dto, CancellationToken cancellationToken = default);

    Task EliminarAsync(Guid usuarioId, Guid id, CancellationToken cancellationToken = default);
}
