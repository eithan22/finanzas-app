using Finanzas.Application.Dtos;
using Finanzas.Domain.Entidades;

namespace Finanzas.Application.Interfaces.IMapper;


// Arma las categorías con las que arranca toda cuenta nueva (RF-26).
// No persiste nada: solo construye la lista.

public interface IMapperCategoria
{
    IReadOnlyList<Categoria> CrearPorDefecto(Guid usuarioId);

    Categoria ACrear(Guid usuarioId, CrearCategoriaDto dto);

    void AplicarCambios(Categoria categoria, ActualizarCategoriaDto dto);

    CategoriaResponseDto AResponseDto(Categoria categoria);
}
