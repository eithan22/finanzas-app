using FluentValidation;

namespace Finanzas.Application.Dtos;

public class EliminarCuentaDtoValidador : AbstractValidator<EliminarCuentaDto>
{
    public EliminarCuentaDtoValidador()
    {
        RuleFor(x => x.Password).NotEmpty();
    }
}
