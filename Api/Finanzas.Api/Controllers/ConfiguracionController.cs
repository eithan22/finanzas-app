using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finanzas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/configuracion")]
public class ConfiguracionController : ControllerBase
{
    private readonly IServicioConfiguracion _servicioConfiguracion;
    private readonly IUsuarioActualService _usuarioActual;

    public ConfiguracionController(IServicioConfiguracion servicioConfiguracion, IUsuarioActualService usuarioActual)
    {
        _servicioConfiguracion = servicioConfiguracion;
        _usuarioActual = usuarioActual;
    }

    [HttpGet]
    public async Task<ActionResult<ConfiguracionResponseDto>> Obtener(CancellationToken cancellationToken)
    {
        var resultado = await _servicioConfiguracion.ObtenerAsync(_usuarioActual.UsuarioId, cancellationToken);
        return Ok(resultado);
    }

    [HttpPut]
    public async Task<ActionResult<ConfiguracionResponseDto>> Actualizar(ActualizarConfiguracionDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _servicioConfiguracion.ActualizarAsync(_usuarioActual.UsuarioId, dto, cancellationToken);
        return Ok(resultado);
    }
}
