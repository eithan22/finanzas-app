namespace Finanzas.Domain.Excepciones;


// Se lanza al intentar registrar una cuenta con un email que ya existe
// (RF-26). El índice único sobre NormalizedEmail lo garantiza a nivel de
// base, esta excepción es la validación de negocio previa al INSERT.

public class EmailYaRegistradoException : DomainException
{
    public EmailYaRegistradoException(string email)
        : base($"Ya existe una cuenta registrada con el email {email}.")
    {
    }
}
