namespace Finanzas.Domain.Excepciones;


// Email inexistente o contraseña incorrecta: mismo mensaje para las dos
// (anti-enumeración), mismo criterio que RecursoNoEncontradoException.

public class CredencialesInvalidasException : DomainException
{
    public CredencialesInvalidasException()
        : base("Email o contraseña incorrectos.")
    {
    }
}
