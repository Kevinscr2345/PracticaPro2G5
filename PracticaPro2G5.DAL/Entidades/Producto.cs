namespace PracticaPro2G5.DAL.Entidades
{
    /// <summary>
    /// Entidad "hija" de la relación 1 -> N. Cada producto pertenece a una única categoría
    /// mediante la llave foránea <see cref="FkCategoria"/>.
    /// </summary>
    public class Producto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        // Llave foránea hacia Categoria.
        public int FkCategoria { get; set; }

        // Propiedad de navegación: el producto pertenece a una categoría.
        public virtual Categoria? Categoria { get; set; }
    }
}
