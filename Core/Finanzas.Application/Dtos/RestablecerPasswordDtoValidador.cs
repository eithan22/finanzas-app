using FluentValidation;

namespace Finanzas.Application.Dtos;

public class RestablecerPasswordDtoValidador : AbstractValidator<RestablecerPasswordDto>
{
    public RestablecerPasswordDtoValidador()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NuevoPassword).ContraseñaSegura();
    }
}
