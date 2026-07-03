using Microsoft.AspNetCore.Mvc;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.BLL.Servicios.Categoria;

namespace PracticaPro2G5.Controllers
{
    /// <summary>
    /// Controlador delgado: valida el ModelState, delega en el servicio y retransmite la
    /// respuesta como JSON. No contiene lógica de negocio.
    /// </summary>
    public class CategoriaController : Controller
    {
        private readonly ICategoriaServicio _categoriaServicio;

        public CategoriaController(ICategoriaServicio categoriaServicio)
        {
            _categoriaServicio = categoriaServicio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var respuesta = await _categoriaServicio.ObtenerTodasAsync();
            return Json(respuesta);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCategoriaPorId(int id)
        {
            var respuesta = await _categoriaServicio.ObtenerPorIdAsync(id);
            return Json(respuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(CategoriaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(RespuestaInvalida());
            }

            var respuesta = await _categoriaServicio.CrearAsync(dto);
            return Json(respuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCategoria(CategoriaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(RespuestaInvalida());
            }

            var respuesta = await _categoriaServicio.ActualizarAsync(dto);
            return Json(respuesta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var respuesta = await _categoriaServicio.EliminarAsync(id);
            return Json(respuesta);
        }

        private Respuesta<CategoriaDto> RespuestaInvalida()
        {
            var errores = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            return new Respuesta<CategoriaDto>
            {
                EsCorrecto = false,
                Mensaje = string.IsNullOrWhiteSpace(errores) ? "Datos inválidos." : errores,
                Codigo = 400
            };
        }
    }
}
