using Finanzas.Domain.Enums;

namespace Finanzas.Domain.Entidades;


// Configuración por usuario. Relación 1:1 con Usuario: la clave primaria es
// el propio UsuarioId, lo que hace imposible a nivel de esquema tener más de
// una configuración por usuario. No hereda de EntidadBase porque no tiene
// un Id autónomo. SRS sección 4. Mono-moneda (restricción 2.4).

public class Configuracion
{
    // PK y FK a la vez: garantiza la relación 1:1 con Usuario.
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    // Moneda única del usuario (ej. "ARS", "USD"). Mono-moneda.
    public string Moneda { get; set; } = string.Empty;

    // Canal(es) de alertas preferido(s) (RF-22, RF-23).
    public CanalAlertas PreferenciaCanalAlertas { get; set; }
}
