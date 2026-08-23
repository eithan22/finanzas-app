using Finanzas.Domain.Common;

namespace Finanzas.Domain.Entidades;


// Usuario de la aplicación.
// El mapeo hacia el mecanismo de autenticación
// (Identity, hashing, tokens) se resuelve en el bloque de Infrastructure/
// Atributos según SRS sección 4: Id, Email, PasswordHash, EmailVerificado,
// FechaCreacion.
public class Usuario : EntidadBase
{
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool EmailVerificado { get; set; }

    public DateTime FechaCreacion { get; set; }

    // Relaciones de navegación (RF-28: todo cuelga del usuario).
   
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

    public Configuracion? Configuracion { get; set; }
}
