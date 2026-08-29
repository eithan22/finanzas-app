using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finanzas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly IServicioCategoria _servicioCategoria;
    private readonly IUsuarioActualService _usuarioActual;

    public CategoriasController(IServicioCategoria servicioCategoria, IUsuarioActualService usuarioActual)
    {
        _servicioCategoria = servicioCategoria;
        _usuarioActual = usuarioActual;
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaResponseDto>> Crear(CrearCategoriaDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _servicioCategoria.CrearAsync(_usuarioActual.UsuarioId, dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = resultado.Id }, resultado);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoriaResponseDto>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var resultado = await _servicioCategoria.ObtenerTodosAsync(_usuarioActual.UsuarioId, cancellationToken);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoriaResponseDto>> ObtenerPorId(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await _servicioCategoria.ObtenerPorIdAsync(_usuarioActual.UsuarioId, id, cancellationToken);
        return Ok(resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoriaResponseDto>> Actualizar(Guid id, ActualizarCategoriaDto dto, CancellationToken cancellationToken)
    {
        var resultado = await _servicioCategoria.ActualizarAsync(_usuarioActual.UsuarioId, id, dto, cancellationToken);
        return Ok(resultado);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id, CancellationToken cancellationToken)
    {
        await _servicioCategoria.EliminarAsync(_usuarioActual.UsuarioId, id, cancellationToken);
        return NoContent();
    }
}
