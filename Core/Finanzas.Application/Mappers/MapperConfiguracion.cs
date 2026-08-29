using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Domain.Entidades;

namespace Finanzas.Application.Mappers;

public class MapperConfiguracion : IMapperConfiguracion
{
    public ConfiguracionResponseDto AResponseDto(Configuracion configuracion) => new()
    {
        Moneda = configuracion.Moneda,
        PreferenciaCanalAlertas = configuracion.PreferenciaCanalAlertas
    };

    public void AplicarCambios(Configuracion configuracion, ActualizarConfiguracionDto dto)
    {
        configuracion.Moneda = dto.Moneda;
        configuracion.PreferenciaCanalAlertas = dto.PreferenciaCanalAlertas;
    }
}
