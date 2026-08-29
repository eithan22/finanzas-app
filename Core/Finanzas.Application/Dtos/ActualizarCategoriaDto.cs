namespace Finanzas.Application.Dtos;


// Tipo no se puede editar a propósito: cambiarlo en una categoría que ya
// tiene subcategorías o transacciones rompería la invariante de RF-06.

public class ActualizarCategoriaDto
{
    public string Nombre { get; set; } = string.Empty;

    public Guid? SubcategoriaDeId { get; set; }
}
