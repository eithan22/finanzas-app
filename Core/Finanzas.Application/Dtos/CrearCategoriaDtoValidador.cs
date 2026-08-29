using FluentValidation;

namespace Finanzas.Application.Dtos;

public class CrearCategoriaDtoValidador : AbstractValidator<CrearCategoriaDto>
{
    public CrearCategoriaDtoValidador()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Tipo).IsInEnum();
    }
}
