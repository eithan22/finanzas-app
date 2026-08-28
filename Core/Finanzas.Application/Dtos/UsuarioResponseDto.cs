namespace Finanzas.Application.Dtos;

public class UsuarioResponseDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool EmailVerificado { get; set; }

    public DateTime FechaCreacion { get; set; }
}
