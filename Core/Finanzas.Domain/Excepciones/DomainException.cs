namespace Finanzas.Domain.Excepciones;


// Excepción base para violaciones de invariantes o reglas de negocio
// del dominio. La capa de Application/Api la captura para traducirla
// a una respuesta apropiada (ej. ProblemDetails 400) en el middleware global.

public abstract class DomainException : Exception
{
    protected DomainException(string mensaje) : base(mensaje)
    {
    }
}
