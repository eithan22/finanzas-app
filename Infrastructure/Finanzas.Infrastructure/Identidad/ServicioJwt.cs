using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Finanzas.Application.Interfaces.IServices;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Finanzas.Infrastructure.Identidad;


// Implementación de IServicioJwt. Lee la configuración vía IConfiguration
// (ya registrado por el framework) en vez de que AddInfrastructure la
// reciba como parámetro: así no cambia esa firma, y este servicio en
// particular sí puede conocer las claves de configuración porque es un
// detalle de implementación propio, no algo que Application necesite saber.

internal sealed class ServicioJwt : IServicioJwt
{
    private readonly IConfiguration _configuracion;

    public ServicioJwt(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public string GenerarTokenAcceso(Guid usuarioId, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new Claim(ClaimTypes.Email, email)
        };

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuracion["Jwt:Key"]!));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuracion["Jwt:Issuer"],
            audience: _configuracion["Jwt:Audience"],
            claims: claims,
            expires: ObtenerExpiracionAcceso(),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerarRefreshToken() =>
        // Random criptográfico, opaco: no es un JWT, se guarda y se compara.
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public DateTime ObtenerExpiracionAcceso() =>
        DateTime.UtcNow.AddMinutes(double.Parse(_configuracion["Jwt:ExpiracionMinutosAcceso"] ?? "15"));

    public DateTime ObtenerExpiracionRefresh() =>
        DateTime.UtcNow.AddDays(double.Parse(_configuracion["Jwt:ExpiracionDiasRefresh"] ?? "30"));
}
