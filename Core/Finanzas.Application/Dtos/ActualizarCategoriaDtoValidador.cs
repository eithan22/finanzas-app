using FluentValidation;

namespace Finanzas.Application.Dtos;

public class ActualizarCategoriaDtoValidador : AbstractValidator<ActualizarCategoriaDto>
{
    public ActualizarCategoriaDtoValidador()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
    }
}
