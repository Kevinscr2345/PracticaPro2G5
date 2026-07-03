using System.ComponentModel.DataAnnotations;

namespace PracticaPro2G5.BLL.Dtos
{
    /// <summary>
    /// Objeto de transferencia de datos de Producto. Incluye la llave foránea de la
    /// categoría (combo box) y el nombre de la categoría para mostrarlo en la tabla.
    /// </summary>
    public class ProductoDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int FkCategoria { get; set; }

        // Solo lectura: se llena al mapear desde la entidad para mostrarlo en la tabla.
        public string? NombreCategoria { get; set; }
    }
}
