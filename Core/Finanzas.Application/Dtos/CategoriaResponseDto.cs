using Finanzas.Domain.Enums;

namespace Finanzas.Application.Dtos;

public class CategoriaResponseDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public TipoCategoria Tipo { get; set; }

    public Guid? SubcategoriaDeId { get; set; }
}
