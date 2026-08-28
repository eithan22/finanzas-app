using Finanzas.Domain.Entidades;
using Finanzas.Domain.Interfaces;
using Finanzas.Infrastructure.Persistencia.Mapeos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.Infrastructure.Persistencia.Repositorios;


// Único punto del sistema que conoce a la vez Domain.Usuario y ApplicationUser
// (Identity). Hacia afuera solo recibe y devuelve Usuario de dominio.

// No hereda de RepositorioBase<T> a propósito: sus métodos de escritura no
// operan sobre la entidad de dominio sino sobre la fila de Identity, así que
// necesitan implementación propia.

public class UsuarioRepository : IUsuarioRepository
{
    private readonly FinanzasDbContext _contexto;

    // Normalizador propio de Identity. Se inyecta en vez de escribir un
    // ToUpper() a mano para que las búsquedas por email coincidan exactamente
    // con las que hace UserManager en el flujo de login (RF-27).
    private readonly ILookupNormalizer _normalizador;

    public UsuarioRepository(FinanzasDbContext contexto, ILookupNormalizer normalizador)
    {
        _contexto = contexto;
        _normalizador = normalizador;
    }

    public async Task<Usuario?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appUser = await _contexto.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return appUser is null ? null : UsuarioMapper.ADominio(appUser);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailNormalizado = _normalizador.NormalizeEmail(email);

        var appUser = await _contexto.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == emailNormalizado, cancellationToken);

        return appUser is null ? null : UsuarioMapper.ADominio(appUser);
    }

    public Task<bool> ExisteEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var emailNormalizado = _normalizador.NormalizeEmail(email);

        return _contexto.Users
            .AnyAsync(u => u.NormalizedEmail == emailNormalizado, cancellationToken);
    }

    public async Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        // El Id se fija acá en vez de dejárselo generar a EF: la fila que se
        // inserta es otro objeto (ApplicationUser), así que el Id generado
        // quedaría en ese objeto y el Usuario del que llama seguiría con
        // Guid.Empty. Si el cliente ya mandó un GUID propio (RF-33), se respeta.
        if (usuario.Id == Guid.Empty)
        {
            usuario.Id = Guid.NewGuid();
        }

        if (usuario.FechaCreacion == default)
        {
            usuario.FechaCreacion = DateTime.UtcNow;
        }

        var appUser = UsuarioMapper.ANuevoIdentity(usuario, _normalizador);

        await _contexto.Users.AddAsync(appUser, cancellationToken);
    }

    public void Actualizar(Usuario usuario)
    {
        // Find revisa primero lo que el contexto ya tiene cargado y solo va a
        // la base si el usuario no fue leído antes en esta misma operación,
        // caso poco habitual: para actualizar algo, antes hubo que leerlo.
        var appUser = _contexto.Users.Find(usuario.Id)
            ?? throw new InvalidOperationException(
                $"No existe un usuario con Id {usuario.Id} para actualizar.");

        UsuarioMapper.VolcarCambios(usuario, appUser, _normalizador);
    }

    public void Eliminar(Usuario usuario)
    {
        // RF-31: al borrar la fila del usuario, sus categorías y su
        // configuración caen por la cascada definida en el modelo.
        var appUser = _contexto.Users.Find(usuario.Id)
            ?? throw new InvalidOperationException(
                $"No existe un usuario con Id {usuario.Id} para eliminar.");

        _contexto.Users.Remove(appUser);
    }
}
