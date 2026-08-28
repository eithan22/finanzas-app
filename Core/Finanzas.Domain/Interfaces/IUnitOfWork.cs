namespace Finanzas.Domain.Interfaces;


// Confirma contra la base de datos, de una sola vez, todos los cambios que
// los repositorios marcaron con AgregarAsync / Actualizar / Eliminar.

// Está separado de IRepositorioBase a propósito: así un caso de uso que toca
// varias entidades (ej. RF-24, que crea el Usuario, su Configuracion y sus
// categorías por defecto) guarda todo junto o no guarda nada, en vez de
// hacer varios guardados sueltos que podrían fallar a la mitad.

public interface IUnitOfWork
{
    Task<int> GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
