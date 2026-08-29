using Finanzas.Application.Dtos;
using Finanzas.Domain.Entidades;

namespace Finanzas.Application.Interfaces.IMapper;

public interface IMapperConfiguracion
{
    ConfiguracionResponseDto AResponseDto(Configuracion configuracion);

    void AplicarCambios(Configuracion configuracion, ActualizarConfiguracionDto dto);
}
