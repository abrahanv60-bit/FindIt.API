
using Microsoft.EntityFrameworkCore;
using FindIt.API.Models;

namespace FindIt.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Objeto> Objetos { get; set; } = null!;
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Solicitud> Solicitudes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Objeto>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(o => o.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Solicitud>()
                .HasOne<Objeto>()
                .WithMany()
                .HasForeignKey(s => s.ObjetoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
