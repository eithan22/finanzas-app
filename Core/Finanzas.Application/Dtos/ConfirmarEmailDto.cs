namespace Finanzas.Application.Dtos;

public class ConfirmarEmailDto
{
    public Guid UsuarioId { get; set; }

    public string Token { get; set; } = string.Empty;
}
