using Finanzas.Application.Dtos;

namespace Finanzas.Application.Interfaces.IServices;

public interface IAuthService
{
    Task<UsuarioResponseDto> RegistrarAsync(RegistrarUsuarioDto dto, CancellationToken cancellationToken = default);
}
