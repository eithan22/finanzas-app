namespace Finanzas.Domain.Excepciones;


// Se lanza cuando se pide un recurso que no existe, o que existe pero es de
// otro usuario — desde afuera son indistinguibles a propósito (RF-28): decir
// "existe pero no es tuyo" ya sería filtrar información.

public class RecursoNoEncontradoException : DomainException
{
    public RecursoNoEncontradoException(string recurso, Guid id)
        : base($"No se encontró {recurso} con Id {id}.")
    {
    }
}
