using FluentValidation;
using Finanzas.Application.Dtos;
using Finanzas.Domain.Entidades;
using Finanzas.Domain.Enums;
using Finanzas.Domain.Excepciones;
using Finanzas.Domain.Interfaces;
using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Application.Interfaces.IServices;

namespace Finanzas.Application.Servicios;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguracionRepository _configuracionRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServicioHashPassword _servicioHashPassword;
    private readonly IServicioVerificacionEmail _servicioVerificacionEmail;
    private readonly IMapperUsuario _mapperUsuario;
    private readonly IMapperCategoria _mapperCategoria;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidador;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IConfiguracionRepository configuracionRepository,
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork,
        IServicioHashPassword servicioHashPassword,
        IServicioVerificacionEmail servicioVerificacionEmail,
        IMapperUsuario mapperUsuario,
        IMapperCategoria mapperCategoria,
        IValidator<RegistrarUsuarioDto> registrarValidador)
    {
        _usuarioRepository = usuarioRepository;
        _configuracionRepository = configuracionRepository;
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
        _servicioHashPassword = servicioHashPassword;
        _servicioVerificacionEmail = servicioVerificacionEmail;
        _mapperUsuario = mapperUsuario;
        _mapperCategoria = mapperCategoria;
        _registrarValidador = registrarValidador;
    }

    public async Task<UsuarioResponseDto> RegistrarAsync(RegistrarUsuarioDto dto, CancellationToken cancellationToken = default)
    {
        await _registrarValidador.ValidateAndThrowAsync(dto, cancellationToken);

        if (await _usuarioRepository.ExisteEmailAsync(dto.Email, cancellationToken))
        {
            throw new EmailYaRegistradoException(dto.Email);
        }

        var passwordHasheado = _servicioHashPassword.Hashear(dto.Password);
        var usuario = _mapperUsuario.ARegistro(dto, passwordHasheado);

        // AgregarAsync completa usuario.Id y usuario.FechaCreacion.
        await _usuarioRepository.AgregarAsync(usuario, cancellationToken);

        await _configuracionRepository.AgregarAsync(new Configuracion
        {
            UsuarioId = usuario.Id,
            Moneda = "ARS",
            PreferenciaCanalAlertas = CanalAlertas.Email
        }, cancellationToken);

        var categoriasPorDefecto = _mapperCategoria.CrearPorDefecto(usuario.Id);
        await _categoriaRepository.AgregarVariasAsync(categoriasPorDefecto, cancellationToken);

        // Todo lo de arriba solo está marcado en memoria: acá se manda como
        // una sola transacción (Usuario + Configuracion + categorías).
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        // TODO(Fase 4 - proveedor de email pendiente de elegir): enviar el
        // token generado al email del usuario. Por ahora la cuenta queda
        // creada con EmailVerificado = false y sin notificación real.
        await _servicioVerificacionEmail.GenerarTokenAsync(usuario.Id, cancellationToken);

        return _mapperUsuario.AResponseDto(usuario);
    }
}
