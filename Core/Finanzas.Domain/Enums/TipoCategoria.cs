namespace Finanzas.Domain.Enums;


// Tipo de una categoría, que determina si aplica a ingresos o a gastos.
// Una transacción y su categoría deben compartir el mismo tipo (RF-06).

public enum TipoCategoria
{
    Ingreso = 1,
    Gasto = 2
}
