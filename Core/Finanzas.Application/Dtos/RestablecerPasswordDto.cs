namespace Finanzas.Application.Dtos;

public class RestablecerPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NuevoPassword { get; set; } = string.Empty;
}
