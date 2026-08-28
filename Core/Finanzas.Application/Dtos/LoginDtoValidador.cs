using FluentValidation;

namespace Finanzas.Application.Dtos;


// Solo "no vacío": la política de contraseña (RF-30) ya se validó al
// registrar, acá solo hace falta algo para comparar contra el hash.

public class LoginDtoValidador : AbstractValidator<LoginDto>
{
    public LoginDtoValidador()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
