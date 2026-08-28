using Finanzas.Application.Dtos;

namespace Finanzas.Application.Interfaces.IServices;

public interface IAuthService
{
    Task<UsuarioResponseDto> RegistrarAsync(RegistrarUsuarioDto dto, CancellationToken cancellationToken = default);

    Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);

    Task<LoginResponseDto> RefrescarAsync(string refreshToken, CancellationToken cancellationToken = default);
}
