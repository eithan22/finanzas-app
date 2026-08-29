using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finanzas.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsuarioActualService _usuarioActual;

    public AuthController(IAuthService authService, IUsuarioActualService usuarioActual)
    {
        _authService = authService;
        _usuarioActual = usuarioActual;
    }

    [HttpPost("registro")]
    public async Task<ActionResult<UsuarioResponseDto>> Registrar(RegistrarUsuarioDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _authService.RegistrarAsync(dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, resultado);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _authService.LoginAsync(dto, cancellationToken);
        return Ok(resultado);
    }

    [HttpPost("refrescar")]
    public async Task<ActionResult<LoginResponseDto>> Refrescar(RefrescarTokenDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _authService.RefrescarAsync(dto.RefreshToken, cancellationToken);
        return Ok(resultado);
    }

    [HttpPost("confirmar-email")]
    public async Task<IActionResult> ConfirmarEmail(ConfirmarEmailDto dto, CancellationToken cancellationToken)
    {
        await _authService.ConfirmarEmailAsync(dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("recuperar-password")]
    public async Task<IActionResult> RecuperarPassword(SolicitarRecuperacionDto dto, CancellationToken cancellationToken)
    {
        await _authService.SolicitarRecuperacionAsync(dto, cancellationToken);
        return NoContent();
    }

    [HttpPost("restablecer-password")]
    public async Task<IActionResult> RestablecerPassword(RestablecerPasswordDto dto, CancellationToken cancellationToken)
    {
        await _authService.RestablecerPasswordAsync(dto, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("cuenta")]
    public async Task<IActionResult> EliminarCuenta(EliminarCuentaDto dto, CancellationToken cancellationToken)
    {
        await _authService.EliminarCuentaAsync(_usuarioActual.UsuarioId, dto, cancellationToken);
        return NoContent();
    }
}
