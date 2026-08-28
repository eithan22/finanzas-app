using FluentValidation;

namespace Finanzas.Application.Dtos;


// RF-30, compartida entre el registro y el restablecimiento de contraseña.

public static class ReglasPasswordExtensiones
{
    public static IRuleBuilderOptions<T, string> ContraseñaSegura<T>(this IRuleBuilder<T, string> reglas) =>
        reglas
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("La contraseña debe tener al menos una mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe tener al menos una minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe tener al menos un número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe tener al menos un símbolo.");
}
