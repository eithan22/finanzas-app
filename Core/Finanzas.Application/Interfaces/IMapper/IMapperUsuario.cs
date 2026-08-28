using Finanzas.Application.Dtos;
using Finanzas.Domain.Entidades;

namespace Finanzas.Application.Interfaces.IMapper;

public interface IMapperUsuario
{
    Usuario ARegistro(RegistrarUsuarioDto dto, string passwordHasheado);

    UsuarioResponseDto AResponseDto(Usuario usuario);
}
