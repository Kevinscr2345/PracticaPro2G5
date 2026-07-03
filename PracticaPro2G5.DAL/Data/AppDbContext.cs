using Microsoft.EntityFrameworkCore;
using PracticaPro2G5.DAL.Entidades;

namespace PracticaPro2G5.DAL.Data
{
    /// <summary>
    /// Contexto de EF Core. La cadena de conexión NO se define aquí: se inyecta desde
    /// la capa de presentación (Program.cs) mediante <c>AddDbContext</c>.
    /// </summary>
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categoria> Categorias { get; set; }

        public virtual DbSet<Producto> Productos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // El proveedor (SQLite) se configura externamente en Program.cs.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Nombre)
                      .HasColumnName("Nombre")
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                      .HasColumnName("Descripcion")
                      .HasMaxLength(250);
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Nombre)
                      .HasColumnName("Nombre")
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                      .HasColumnName("Descripcion")
                      .HasMaxLength(250);

                entity.Property(e => e.Precio)
                      .HasColumnName("Precio")
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Stock)
                      .HasColumnName("Stock")
                      .HasDefaultValue(0);

                entity.Property(e => e.FkCategoria)
                      .HasColumnName("FkCategoria");

                // Relación 1 (Categoria) -> N (Producto).
                // Restrict: no permite borrar una categoría que tenga productos asociados.
                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.FkCategoria)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Datos de ejemplo para poblar el combo box desde el primer arranque.
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Instrumental quirúrgico", Descripcion = "Bisturís, pinzas, tijeras y material de quirófano." },
                new Categoria { Id = 2, Nombre = "Material de curación", Descripcion = "Gasas, vendas, apósitos y antisépticos." },
                new Categoria { Id = 3, Nombre = "Equipo de diagnóstico", Descripcion = "Termómetros, tensiómetros y estetoscopios." }
            );

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
