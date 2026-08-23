namespace Finanzas.Domain.Enums;

// Canal(es) por los que el usuario desea recibir alertas (RF-22, RF-23).
// Definido en la Configuración del usuario (SRS sección 4).
public enum CanalAlertas
{
    Push = 1,
    Email = 2,
    Ambos = 3
}
