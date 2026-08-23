using Finanzas.Domain.Common;
using Finanzas.Domain.Enums;
using Finanzas.Domain.Excepciones;

namespace Finanzas.Domain.Entidades;


// Categoría de ingreso o gasto del usuario. Puede ser subcategoría de otra
//categoría (relación autoreferencial vía SubcategoriaDeId), en cuyo caso
//debe compartir el mismo Tipo que su categoría padre (RF-06). 

public class Categoria : EntidadBase
{
    
    // Dueño de la categoría (RF-28: aislamiento por usuario)
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;

    //Ingreso o Gasto. Determina a qué transacciones aplica.
    public TipoCategoria Tipo { get; set; }

    
    // Si tiene valor, esta categoría es subcategoría de otra (autoreferencia).
    // Null si es una categoría de primer nivel.
    
    public Guid? SubcategoriaDeId { get; set; }
    public Categoria? SubcategoriaDe { get; set; }

    // Subcategorías que cuelgan de esta categoría.
    public ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();

    // Asigna esta categoría como subcategoría de <paramref name="padre"/>,
    // validando la invariante RF-06 (mismo Tipo que el padre).
    public void AsignarComoSubcategoriaDe(Categoria padre)
    {
        if (padre.Tipo != Tipo)
        {
            throw new CategoriaInvalidaException(
                "Una subcategoría debe tener el mismo Tipo (Ingreso/Gasto) que su categoría padre.");
        }

        SubcategoriaDeId = padre.Id;
        SubcategoriaDe = padre;
    }
}
