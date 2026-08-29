using Finanzas.Application.Dtos;
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

    public Categoria ACrear(Guid usuarioId, CrearCategoriaDto dto) => new()
    {
        Id = Guid.NewGuid(),
        UsuarioId = usuarioId,
        Nombre = dto.Nombre,
        Tipo = dto.Tipo
    };

    public void AplicarCambios(Categoria categoria, ActualizarCategoriaDto dto)
    {
        categoria.Nombre = dto.Nombre;
    }

    public CategoriaResponseDto AResponseDto(Categoria categoria) => new()
    {
        Id = categoria.Id,
        Nombre = categoria.Nombre,
        Tipo = categoria.Tipo,
        SubcategoriaDeId = categoria.SubcategoriaDeId
    };
}
