using Finanzas.Domain.Entidades;
using Finanzas.Infrastructure.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanzas.Infrastructure.Persistencia.Configuraciones;


// Mapeo de Categoria a tabla. Va acá, con Fluent API, y no con atributos en
// la entidad, para que Finanzas.Domain no tenga que referenciar EF Core.

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        // El enum se guarda como texto ("Ingreso" / "Gasto") para que la
        // tabla se pueda leer sin traducir números a mano.
        builder.Property(c => c.Tipo)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        // Dueño de la categoría. La FK apunta a la tabla de Identity, sin
        // propiedad de navegación: Domain.Usuario no es una entidad de EF,
        // la única entidad mapeada a AspNetUsers es ApplicationUser.
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // Autoreferencia para subcategorías (RF-06). Restrict y no Cascade
        // porque SQL Server rechaza varios caminos de borrado en cascada
        // sobre la misma tabla.
        builder.HasOne(c => c.SubcategoriaDe)
            .WithMany(c => c.Subcategorias)
            .HasForeignKey(c => c.SubcategoriaDeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Toda consulta de categorías filtra por usuario (RF-28) y muchas
        // además por tipo, así que el índice cubre las dos columnas.
        builder.HasIndex(c => new { c.UsuarioId, c.Tipo });

        // Un usuario no puede tener dos categorías con el mismo nombre en el
        // mismo nivel.
        // HasFilter(null) es imprescindible acá: como SubcategoriaDeId es
        // nullable, EF por defecto le agrega un "WHERE SubcategoriaDeId IS NOT
        // NULL" al índice, y eso dejaría afuera justamente a las categorías de
        // primer nivel, que son el caso más común. Sin filtro, SQL Server
        // compara los NULL entre sí como iguales, que es lo que queremos.
        builder.HasIndex(c => new { c.UsuarioId, c.Nombre, c.SubcategoriaDeId })
            .IsUnique()
            .HasFilter(null);
    }
}
