namespace Finanzas.Domain.Excepciones;

public class RefreshTokenInvalidoException : DomainException
{
    public RefreshTokenInvalidoException()
        : base("El refresh token no es válido, está vencido o fue revocado.")
    {
    }
}
