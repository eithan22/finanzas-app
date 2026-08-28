using Finanzas.Domain.Entidades;
using Finanzas.Infrastructure.Identidad;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.Infrastructure.Persistencia;


// Contexto de EF Core. Hereda de IdentityUserContext y no de IdentityDbContext
// porque el SRS no define roles: así se crean las tablas de usuarios, claims,
// logins y tokens de Identity, pero no las tres de roles, que quedarían
// vacías para siempre. Si algún día hacen falta, se agregan por migración.

public class FinanzasDbContext : IdentityUserContext<ApplicationUser, Guid>
{
    public FinanzasDbContext(DbContextOptions<FinanzasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Configuracion> Configuraciones => Set<Configuracion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Primero el mapeo de Identity (tablas AspNet*), después el nuestro.
        base.OnModelCreating(modelBuilder);

        // Un email, una cuenta (RF-24), garantizado por la base de datos.
        // Identity trae este índice pero NO único: la unicidad la valida
        // UserManager con RequireUniqueEmail. Como UsuarioRepository escribe
        // con EF directo y no pasa por UserManager, sin esto un doble submit
        // del registro o dos requests en paralelo crearían dos cuentas con el
        // mismo email. HasFilter(null) por el mismo motivo que en Categorias:
        // NormalizedEmail es nullable y EF le pondría un filtro por su cuenta.
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasFilter(null);

        // Toma todas las clases IEntityTypeConfiguration de este proyecto,
        // así no hay que registrarlas una por una acá.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanzasDbContext).Assembly);
    }
}
