namespace Finanzas.Domain.Interfaces;

// Operaciones de escritura comunes a todos los repositorios: agregar,
// marcar como actualizada y marcar como eliminada una entidad ya cargada.

// No incluye métodos de búsqueda/lectura porque esos varían según la
// entidad (algunas necesitan filtrar por usuarioId por seguridad, otras
// se buscan por email, etc.) — unificarlos ahí sería forzar una forma
// común donde no la hay.


public interface IRepositorioBase<TEntidad> where TEntidad : class
{  
    Task AgregarAsync(TEntidad entidad, CancellationToken cancellationToken = default);

    //Marca una entidad ya cargada como modificada.
    void Actualizar(TEntidad entidad);

    //Marca una entidad ya cargada como eliminada.
    void Eliminar(TEntidad entidad);
}
