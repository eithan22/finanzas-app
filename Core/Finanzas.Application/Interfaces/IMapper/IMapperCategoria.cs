using Finanzas.Domain.Entidades;

namespace Finanzas.Application.Interfaces.IMapper;


// Arma las categorías con las que arranca toda cuenta nueva (RF-26).
// No persiste nada: solo construye la lista.

public interface IMapperCategoria
{
    IReadOnlyList<Categoria> CrearPorDefecto(Guid usuarioId);
}
