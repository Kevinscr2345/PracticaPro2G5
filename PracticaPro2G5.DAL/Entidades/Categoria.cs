namespace PracticaPro2G5.DAL.Entidades
{
    /// <summary>
    /// Entidad "padre" de la relación 1 -> N. Una categoría agrupa varios productos médicos.
    /// POCO simple: las validaciones de UI viven en los DTOs, no en la entidad.
    /// </summary>
    public class Categoria
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        // Propiedad de navegación: una categoría tiene muchos productos.
        public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
