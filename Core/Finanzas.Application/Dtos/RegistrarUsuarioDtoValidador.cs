using FluentValidation;

namespace Finanzas.Application.Dtos;


// RF-30: política de contraseñas. Identity ya la exige, pero
// UsuarioRepository no pasa por UserManager al escribir, así que hay que
// validarla también acá.

public class RegistrarUsuarioDtoValidador : AbstractValidator<RegistrarUsuarioDto>
{
    public RegistrarUsuarioDtoValidador()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe tener al menos una minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener al menos un símbolo.");
    }
}
