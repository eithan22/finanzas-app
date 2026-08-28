namespace Finanzas.Domain.Excepciones;


// Credenciales correctas pero el email todavía no se confirmó (RF-26
// bloquea operaciones hasta confirmar). Mensaje distinto de
// CredencialesInvalidasException a propósito: acá sí es seguro decirle al
// usuario qué le falta, porque ya demostró que sabe su contraseña.

public class CuentaNoVerificadaException : DomainException
{
    public CuentaNoVerificadaException()
        : base("Tenés que verificar tu email antes de iniciar sesión.")
    {
    }
}
