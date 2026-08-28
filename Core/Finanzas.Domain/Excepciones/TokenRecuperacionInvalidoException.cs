namespace Finanzas.Domain.Excepciones;

public class TokenRecuperacionInvalidoException : DomainException
{
    public TokenRecuperacionInvalidoException()
        : base("El token de recuperación no es válido o está vencido.")
    {
    }
}
