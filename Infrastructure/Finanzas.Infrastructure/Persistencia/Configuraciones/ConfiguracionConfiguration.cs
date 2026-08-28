using Finanzas.Domain.Entidades;
using Finanzas.Infrastructure.Identidad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finanzas.Infrastructure.Persistencia.Configuraciones;


// Mapeo de Configuracion a tabla. Relación 1:1 con el usuario.

public class ConfiguracionConfiguration : IEntityTypeConfiguration<Configuracion>
{
    public void Configure(EntityTypeBuilder<Configuracion> builder)
    {
        builder.ToTable("Configuraciones");

        // La clave primaria es el propio UsuarioId: a nivel de esquema es
        // imposible que un usuario tenga dos configuraciones.
        builder.HasKey(c => c.UsuarioId);

        // No se autogenera: el valor viene del usuario al que pertenece.
        builder.Property(c => c.UsuarioId)
            .ValueGeneratedNever();

        // Código ISO 4217: "ARS", "USD". Mono-moneda (restricción 2.4).
        builder.Property(c => c.Moneda)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(c => c.PreferenciaCanalAlertas)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Configuracion>(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
