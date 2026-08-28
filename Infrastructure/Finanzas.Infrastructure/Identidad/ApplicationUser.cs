using Microsoft.AspNetCore.Identity;

namespace Finanzas.Infrastructure.Identidad;


// Representación del usuario para ASP.NET Identity: mapea a la tabla
// AspNetUsers y trae de fábrica el hash de contraseña, los stamps de
// seguridad, los tokens de verificación y recuperación, y el bloqueo por
// intentos fallidos (RF-25, RF-27, RF-29, RF-30).

// El dominio NO conoce esta clase: trabaja siempre con Domain.Usuario. La
// traducción entre las dos vive en UsuarioMapper, dentro de Infrastructure.

public class ApplicationUser : IdentityUser<Guid>
{
    // Identity no trae fecha de alta y el SRS (sección 4) sí la pide.
    public DateTime FechaCreacion { get; set; }
}
