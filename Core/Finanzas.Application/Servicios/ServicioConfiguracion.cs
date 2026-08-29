using FluentValidation;
using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Application.Interfaces.IServices;
using Finanzas.Domain.Entidades;
using Finanzas.Domain.Excepciones;
using Finanzas.Domain.Interfaces;

namespace Finanzas.Application.Servicios;

public class ServicioConfiguracion : IServicioConfiguracion
{
    private readonly IConfiguracionRepository _configuracionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapperConfiguracion _mapperConfiguracion;
    private readonly IValidator<ActualizarConfiguracionDto> _actualizarValidador;

    public ServicioConfiguracion(
        IConfiguracionRepository configuracionRepository,
        IUnitOfWork unitOfWork,
        IMapperConfiguracion mapperConfiguracion,
        IValidator<ActualizarConfiguracionDto> actualizarValidador)
    {
        _configuracionRepository = configuracionRepository;
        _unitOfWork = unitOfWork;
        _mapperConfiguracion = mapperConfiguracion;
        _actualizarValidador = actualizarValidador;
    }

    public async Task<ConfiguracionResponseDto> ObtenerAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var configuracion = await _configuracionRepository.ObtenerPorUsuarioAsync(usuarioId, cancellationToken)
            ?? throw new RecursoNoEncontradoException(nameof(Configuracion), usuarioId);

        return _mapperConfiguracion.AResponseDto(configuracion);
    }

    public async Task<ConfiguracionResponseDto> ActualizarAsync(Guid usuarioId, ActualizarConfiguracionDto dto, CancellationToken cancellationToken = default)
    {
        await _actualizarValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var configuracion = await _configuracionRepository.ObtenerPorUsuarioAsync(usuarioId, cancellationToken)
            ?? throw new RecursoNoEncontradoException(nameof(Configuracion), usuarioId);

        _mapperConfiguracion.AplicarCambios(configuracion, dto);

        _configuracionRepository.Actualizar(configuracion);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return _mapperConfiguracion.AResponseDto(configuracion);
    }
}
