namespace Finanzas.Domain.Excepciones;


// Se lanza cuando se viola una invariante de Categoría. Cubre, entre otras,
// la regla RF-06 del SRS: una subcategoría debe tener el mismo Tipo
// (Ingreso/Gasto) que su categoría padre.

public class CategoriaInvalidaException : DomainException
{
    public CategoriaInvalidaException(string mensaje) : base(mensaje)
    {
    }
}
