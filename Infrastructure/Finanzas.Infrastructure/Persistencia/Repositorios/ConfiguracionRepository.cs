using Finanzas.Domain.Entidades;
using Finanzas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.Infrastructure.Persistencia.Repositorios;


// Acceso a datos de la Configuración del usuario (1:1). Como la clave
// primaria es el UsuarioId, buscar por usuario es buscar por clave.

public class ConfiguracionRepository : RepositorioBase<Configuracion>, IConfiguracionRepository
{
    public ConfiguracionRepository(FinanzasDbContext contexto) : base(contexto)
    {
    }

    public Task<Configuracion?> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return Contexto.Configuraciones
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId, cancellationToken);
    }
}
