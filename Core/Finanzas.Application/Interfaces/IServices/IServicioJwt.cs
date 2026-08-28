namespace Finanzas.Application.Interfaces.IServices;

public interface IServicioJwt
{
    string GenerarTokenAcceso(Guid usuarioId, string email);

    string GenerarRefreshToken();

    DateTime ObtenerExpiracionAcceso();

    DateTime ObtenerExpiracionRefresh();
}
