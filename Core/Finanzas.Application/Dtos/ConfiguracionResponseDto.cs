using Finanzas.Domain.Enums;

namespace Finanzas.Application.Dtos;

public class ConfiguracionResponseDto
{
    public string Moneda { get; set; } = string.Empty;

    public CanalAlertas PreferenciaCanalAlertas { get; set; }
}
