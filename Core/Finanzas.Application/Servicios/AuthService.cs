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
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IServicioHashPassword _servicioHashPassword;
    private readonly IServicioVerificacionEmail _servicioVerificacionEmail;
    private readonly IServicioJwt _servicioJwt;
    private readonly IServicioRecuperacionPassword _servicioRecuperacionPassword;
    private readonly IMapperUsuario _mapperUsuario;
    private readonly IMapperCategoria _mapperCategoria;
    private readonly IValidator<RegistrarUsuarioDto> _registrarValidador;
    private readonly IValidator<LoginDto> _loginValidador;
    private readonly IValidator<ConfirmarEmailDto> _confirmarEmailValidador;
    private readonly IValidator<SolicitarRecuperacionDto> _solicitarRecuperacionValidador;
    private readonly IValidator<RestablecerPasswordDto> _restablecerPasswordValidador;
    private readonly IValidator<EliminarCuentaDto> _eliminarCuentaValidador;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IConfiguracionRepository configuracionRepository,
        ICategoriaRepository categoriaRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IServicioHashPassword servicioHashPassword,
        IServicioVerificacionEmail servicioVerificacionEmail,
        IServicioJwt servicioJwt,
        IServicioRecuperacionPassword servicioRecuperacionPassword,
        IMapperUsuario mapperUsuario,
        IMapperCategoria mapperCategoria,
        IValidator<RegistrarUsuarioDto> registrarValidador,
        IValidator<LoginDto> loginValidador,
        IValidator<ConfirmarEmailDto> confirmarEmailValidador,
        IValidator<SolicitarRecuperacionDto> solicitarRecuperacionValidador,
        IValidator<RestablecerPasswordDto> restablecerPasswordValidador,
        IValidator<EliminarCuentaDto> eliminarCuentaValidador)
    {
        _usuarioRepository = usuarioRepository;
        _configuracionRepository = configuracionRepository;
        _categoriaRepository = categoriaRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _servicioHashPassword = servicioHashPassword;
        _servicioVerificacionEmail = servicioVerificacionEmail;
        _servicioJwt = servicioJwt;
        _servicioRecuperacionPassword = servicioRecuperacionPassword;
        _mapperUsuario = mapperUsuario;
        _mapperCategoria = mapperCategoria;
        _registrarValidador = registrarValidador;
        _loginValidador = loginValidador;
        _confirmarEmailValidador = confirmarEmailValidador;
        _solicitarRecuperacionValidador = solicitarRecuperacionValidador;
        _restablecerPasswordValidador = restablecerPasswordValidador;
        _eliminarCuentaValidador = eliminarCuentaValidador;
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

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        await _loginValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email, cancellationToken);
        if (usuario is null || !_servicioHashPassword.Verificar(dto.Password, usuario.PasswordHash))
        {
            throw new CredencialesInvalidasException();
        }

        if (!usuario.EmailVerificado)
        {
            throw new CuentaNoVerificadaException();
        }

        return await EmitirTokensAsync(usuario, cancellationToken);
    }

    public async Task<LoginResponseDto> RefrescarAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenGuardado = await _refreshTokenRepository.ObtenerPorTokenAsync(refreshToken, cancellationToken);
        if (tokenGuardado is null || tokenGuardado.Revocado || tokenGuardado.FechaExpiracion <= DateTime.UtcNow)
        {
            throw new RefreshTokenInvalidoException();
        }

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(tokenGuardado.UsuarioId, cancellationToken)
            ?? throw new RefreshTokenInvalidoException();

        // Rotación: el token usado queda inválido, se emite uno nuevo.
        tokenGuardado.Revocado = true;
        _refreshTokenRepository.Actualizar(tokenGuardado);

        return await EmitirTokensAsync(usuario, cancellationToken);
    }

    private async Task<LoginResponseDto> EmitirTokensAsync(Usuario usuario, CancellationToken cancellationToken)
    {
        var tokenAcceso = _servicioJwt.GenerarTokenAcceso(usuario.Id, usuario.Email);
        var refreshToken = _servicioJwt.GenerarRefreshToken();
        var expiraEn = _servicioJwt.ObtenerExpiracionAcceso();

        await _refreshTokenRepository.AgregarAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Token = refreshToken,
            FechaCreacion = DateTime.UtcNow,
            FechaExpiracion = _servicioJwt.ObtenerExpiracionRefresh(),
            Revocado = false
        }, cancellationToken);

        await _unitOfWork.GuardarCambiosAsync(cancellationToken);

        return new LoginResponseDto
        {
            TokenAcceso = tokenAcceso,
            ExpiraEn = expiraEn,
            RefreshToken = refreshToken
        };
    }

    public async Task ConfirmarEmailAsync(ConfirmarEmailDto dto, CancellationToken cancellationToken = default)
    {
        await _confirmarEmailValidador.ValidateAndThrowAsync(dto, cancellationToken);

        // Si el UsuarioId no existe, ConfirmarAsync (Infrastructure) tiraría
        // una InvalidOperationException que caería como 500 — se detecta acá
        // antes y se devuelve el mismo "token inválido" que un token vencido,
        // mismo criterio anti-enumeración que ya usa la recuperación de contraseña.
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(dto.UsuarioId, cancellationToken);
        if (usuario is null)
        {
            throw new TokenVerificacionInvalidoException();
        }

        var exito = await _servicioVerificacionEmail.ConfirmarAsync(dto.UsuarioId, dto.Token, cancellationToken);
        if (!exito)
        {
            throw new TokenVerificacionInvalidoException();
        }
    }

    public async Task SolicitarRecuperacionAsync(SolicitarRecuperacionDto dto, CancellationToken cancellationToken = default)
    {
        await _solicitarRecuperacionValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email, cancellationToken);
        if (usuario is null)
        {
            // Anti-enumeración: no revela si el email existe.
            return;
        }

        // TODO(Fase 4 - proveedor de email pendiente de elegir): enviar el
        // token generado al email del usuario.
        await _servicioRecuperacionPassword.GenerarTokenAsync(usuario.Id, cancellationToken);
    }

    public async Task RestablecerPasswordAsync(RestablecerPasswordDto dto, CancellationToken cancellationToken = default)
    {
        await _restablecerPasswordValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email, cancellationToken)
            ?? throw new TokenRecuperacionInvalidoException();

        var exito = await _servicioRecuperacionPassword.RestablecerAsync(
            usuario.Id, dto.Token, dto.NuevoPassword, cancellationToken);

        if (!exito)
        {
            throw new TokenRecuperacionInvalidoException();
        }
    }

    public async Task EliminarCuentaAsync(Guid usuarioId, EliminarCuentaDto dto, CancellationToken cancellationToken = default)
    {
        await _eliminarCuentaValidador.ValidateAndThrowAsync(dto, cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(usuarioId, cancellationToken)
            ?? throw new RecursoNoEncontradoException(nameof(Usuario), usuarioId);

        if (!_servicioHashPassword.Verificar(dto.Password, usuario.PasswordHash))
        {
            throw new CredencialesInvalidasException();
        }

        // Borra solo la fila del usuario: Categoria, Configuracion y
        // RefreshTokens caen por la cascada definida en el modelo (RF-31).
        _usuarioRepository.Eliminar(usuario);
        await _unitOfWork.GuardarCambiosAsync(cancellationToken);
    }
}
