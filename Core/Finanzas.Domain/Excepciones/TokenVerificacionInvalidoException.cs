namespace Finanzas.Domain.Excepciones;

public class TokenVerificacionInvalidoException : DomainException
{
    public TokenVerificacionInvalidoException()
        : base("El token de verificación no es válido o está vencido.")
    {
    }
}
