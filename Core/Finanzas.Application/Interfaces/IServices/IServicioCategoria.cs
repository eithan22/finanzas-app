using Finanzas.Application.Dtos;

namespace Finanzas.Application.Interfaces.IServices;

public interface IServicioCategoria : IServicioBase<CategoriaResponseDto, CrearCategoriaDto, ActualizarCategoriaDto>
{
}
