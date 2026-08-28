using FluentValidation;

namespace Finanzas.Application.Dtos;

public class SolicitarRecuperacionDtoValidador : AbstractValidator<SolicitarRecuperacionDto>
{
    public SolicitarRecuperacionDtoValidador()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
