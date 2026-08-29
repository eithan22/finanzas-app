using FluentValidation;

namespace Finanzas.Application.Dtos;

public class ConfirmarEmailDtoValidador : AbstractValidator<ConfirmarEmailDto>
{
    public ConfirmarEmailDtoValidador()
    {
        RuleFor(x => x.UsuarioId).NotEmpty();
        RuleFor(x => x.Token).NotEmpty();
    }
}
