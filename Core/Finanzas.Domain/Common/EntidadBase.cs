namespace Finanzas.Domain.Common;

// Clase base para entidades con identidad propia de tipo Guid.
// Configuracion NO la usa, porque su clave primaria es UsuarioId
// (relación 1:1 estructural con Usuario), no un Id autónomo.

public abstract class EntidadBase
{
    public Guid Id { get; set; }
}
