using Finanzas.Application.Dtos;
using Finanzas.Application.Interfaces.IMapper;
using Finanzas.Domain.Entidades;

namespace Finanzas.Application.Mappers;

public class MapperUsuario : IMapperUsuario
{
    public Usuario ARegistro(RegistrarUsuarioDto dto, string passwordHasheado) => new()
    {
        Email = dto.Email,
        PasswordHash = passwordHasheado
    };

    public UsuarioResponseDto AResponseDto(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Email = usuario.Email,
        EmailVerificado = usuario.EmailVerificado,
        FechaCreacion = usuario.FechaCreacion
    };
}
