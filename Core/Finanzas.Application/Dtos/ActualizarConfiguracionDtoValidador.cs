using FluentValidation;

namespace Finanzas.Application.Dtos;

public class ActualizarConfiguracionDtoValidador : AbstractValidator<ActualizarConfiguracionDto>
{
    public ActualizarConfiguracionDtoValidador()
    {
        RuleFor(x => x.Moneda)
            .NotEmpty()
            .Matches("^[A-Z]{3}$").WithMessage("La moneda debe ser un código ISO 4217 de 3 letras mayúsculas (ej. ARS, USD).");

        RuleFor(x => x.PreferenciaCanalAlertas).IsInEnum();
    }
}
