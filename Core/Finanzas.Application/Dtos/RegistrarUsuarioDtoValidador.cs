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

        RuleFor(x => x.Password).ContraseñaSegura();
    }
}
