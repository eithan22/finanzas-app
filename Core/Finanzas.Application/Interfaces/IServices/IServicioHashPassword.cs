namespace Finanzas.Application.Interfaces.IServices;


// Contrato de hashing de contraseñas. Los servicios de la aplicación piden
// "hasheá esto" o "verificá esto" sin saber qué librería hay detrás.

// Sirve para lo mismo que IUsuarioRepository, pero para otra herramienta: que
// esta capa no dependa de ASP.NET Identity. La implementación vive en
// Infrastructure (RNF-02: las contraseñas nunca se guardan en texto plano).

public interface IServicioHashPassword
{
    // Convierte la contraseña en texto a su hash, listo para guardar.
    string Hashear(string passwordPlano);

    // Compara lo que escribió el usuario contra el hash guardado.
    // Nunca deshace el hash: vuelve a hashear y compara resultados.
    bool Verificar(string passwordPlano, string hashGuardado);
}
