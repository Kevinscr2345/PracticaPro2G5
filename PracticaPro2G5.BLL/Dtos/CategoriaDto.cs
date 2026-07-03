using System.ComponentModel.DataAnnotations;

namespace PracticaPro2G5.BLL.Dtos
{
    /// <summary>
    /// Objeto de transferencia de datos de Categoría. Las validaciones (Data Annotations)
    /// se usan tanto en el servidor (ModelState) como en el cliente (jQuery Unobtrusive).
    /// </summary>
    public class CategoriaDto
    {
        // Nullable para desacoplar la creación (sin Id) de la edición (con Id).
        public int? Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }
    }
}
