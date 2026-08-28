namespace Finanzas.Application.Dtos;

public class LoginResponseDto
{
    public string TokenAcceso { get; set; } = string.Empty;

    public DateTime ExpiraEn { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
}
