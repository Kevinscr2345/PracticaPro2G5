using Microsoft.AspNetCore.Mvc;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.BLL.Servicios.Categoria;
using PracticaPro2G5.BLL.Servicios.Producto;

namespace PracticaPro2G5.Controllers
{
    /// <summary>
    /// Controlador delgado de Productos. Inyecta también el servicio de Categorías para
    /// poblar el combo box de la vista mediante ViewBag.
    /// </summary>
    public class ProductoController : Controller
    {
        private readonly IProductoServicio _productoServicio;
        private readonly ICategoriaServicio _categoriaServicio;

        public ProductoController(IProductoServicio productoServicio, ICategoriaServicio categoriaServicio)
        {
            _productoServicio = productoServicio;
            _categoriaServicio = categoriaServicio;
        }

        public async Task<IActionResult> Index()
        {
            // Carga las categorías para el combo box de los formularios (crear/editar).
            var respuesta = await _categoriaServicio.ObtenerTodasAsync();
            ViewBag.Categorias = respuesta.Dato ?? new List<CategoriaDto>();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos()
        {
            var respuesta = await _productoServicio.ObtenerTodosAsync();
            return Json(respuesta);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductoPorId(int id)
        {
            var respuesta = await _productoServicio.ObtenerPorIdAsync(id);
            return Json(respuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(ProductoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(RespuestaInvalida());
            }

            var respuesta = await _productoServicio.CrearAsync(dto);
            return Json(respuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarProducto(ProductoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(RespuestaInvalida());
            }

            var respuesta = await _productoServicio.ActualizarAsync(dto);
            return Json(respuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var respuesta = await _productoServicio.EliminarAsync(id);
            return Json(respuesta);
        }

        private Respuesta<ProductoDto> RespuestaInvalida()
        {
            var errores = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return new Respuesta<ProductoDto>
            {
                EsCorrecto = false,
                Mensaje = string.IsNullOrWhiteSpace(errores) ? "Datos inválidos." : errores,
                Codigo = 400
            };
        }
    }
}
