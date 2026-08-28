using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Domain.Entidades;
using Finanzas.Domain.Enums;

namespace Finanzas.Application.Mappers;

public class MapperCategoria : IMapperCategoria
{
    private static readonly (string Nombre, TipoCategoria Tipo)[] CategoriasPorDefecto =
    [
        ("Vivienda", TipoCategoria.Gasto),
        ("Supermercado", TipoCategoria.Gasto),
        ("Transporte", TipoCategoria.Gasto),
        ("Servicios", TipoCategoria.Gasto),
        ("Ocio", TipoCategoria.Gasto),
        ("Sueldo", TipoCategoria.Ingreso),
        ("Otros ingresos", TipoCategoria.Ingreso),
    ];

    public IReadOnlyList<Categoria> CrearPorDefecto(Guid usuarioId) =>
        CategoriasPorDefecto
            .Select(c => new Categoria
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Nombre = c.Nombre,
                Tipo = c.Tipo
            })
            .ToList();
}
