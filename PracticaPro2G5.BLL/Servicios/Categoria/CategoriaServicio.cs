using AutoMapper;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.DAL.Repositorios.Generico;
// Alias para distinguir la ENTIDAD (EF Core) del namespace y del DTO homónimos.
using CategoriaEntidad = PracticaPro2G5.DAL.Entidades.Categoria;
using ProductoEntidad = PracticaPro2G5.DAL.Entidades.Producto;

namespace PracticaPro2G5.BLL.Servicios.Categoria
{
    /// <summary>
    /// Lógica de negocio de Categorías. Toda regla y mensaje de error vive aquí; el
    /// controlador solo retransmite la <see cref="Respuesta{T}"/> resultante.
    /// </summary>
    public class CategoriaServicio : ICategoriaServicio
    {
        private readonly IRepositorioGenerico<CategoriaEntidad> _repositorio;
        private readonly IRepositorioGenerico<ProductoEntidad> _repositorioProductos;
        private readonly IMapper _mapper;

        public CategoriaServicio(
            IRepositorioGenerico<CategoriaEntidad> repositorio,
            IRepositorioGenerico<ProductoEntidad> repositorioProductos,
            IMapper mapper)
        {
            _repositorio = repositorio;
            _repositorioProductos = repositorioProductos;
            _mapper = mapper;
        }

        public async Task<Respuesta<IEnumerable<CategoriaDto>>> ObtenerTodasAsync()
        {
            var respuesta = new Respuesta<IEnumerable<CategoriaDto>>();

            var categorias = await _repositorio.ObtenerTodosAsync();
            respuesta.Dato = _mapper.Map<IEnumerable<CategoriaDto>>(categorias);

            return respuesta;
        }

        public async Task<Respuesta<CategoriaDto>> ObtenerPorIdAsync(int id)
        {
            var respuesta = new Respuesta<CategoriaDto>();

            var categoria = await _repositorio.ObtenerPorIdAsync(id);
            if (categoria is null)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "La categoría solicitada no existe.";
                respuesta.Codigo = 404;
                return respuesta;
            }

            respuesta.Dato = _mapper.Map<CategoriaDto>(categoria);
            return respuesta;
        }

        public async Task<Respuesta<CategoriaDto>> CrearAsync(CategoriaDto dto)
        {
            var respuesta = new Respuesta<CategoriaDto>();

            // Regla de negocio: el nombre de la categoría no puede repetirse.
            var existentes = await _repositorio.BuscarAsync(c => c.Nombre.ToLower() == dto.Nombre.ToLower());
            if (existentes.Any())
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = $"Ya existe una categoría con el nombre '{dto.Nombre}'.";
                respuesta.Codigo = 409;
                return respuesta;
            }

            var entidad = _mapper.Map<CategoriaEntidad>(dto);
            await _repositorio.AgregarAsync(entidad);
            await _repositorio.SaveChangesAsync();

            respuesta.Dato = _mapper.Map<CategoriaDto>(entidad);
            respuesta.Mensaje = "Categoría creada correctamente.";
            respuesta.Codigo = 201;
            return respuesta;
        }

        public async Task<Respuesta<CategoriaDto>> ActualizarAsync(CategoriaDto dto)
        {
            var respuesta = new Respuesta<CategoriaDto>();

            if (dto.Id is null or <= 0)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "El identificador de la categoría no es válido.";
                respuesta.Codigo = 400;
                return respuesta;
            }

            var entidad = await _repositorio.ObtenerPorIdAsync(dto.Id.Value);
            if (entidad is null)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "La categoría que intenta actualizar no existe.";
                respuesta.Codigo = 404;
                return respuesta;
            }

            // Regla de negocio: el nombre no puede repetirse en OTRA categoría.
            var duplicadas = await _repositorio.BuscarAsync(
                c => c.Nombre.ToLower() == dto.Nombre.ToLower() && c.Id != dto.Id.Value);
            if (duplicadas.Any())
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = $"Ya existe otra categoría con el nombre '{dto.Nombre}'.";
                respuesta.Codigo = 409;
                return respuesta;
            }

            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;

            await _repositorio.ActualizarAsync(entidad);
            await _repositorio.SaveChangesAsync();

            respuesta.Dato = _mapper.Map<CategoriaDto>(entidad);
            respuesta.Mensaje = "Categoría actualizada correctamente.";
            return respuesta;
        }

        public async Task<Respuesta<bool>> EliminarAsync(int id)
        {
            var respuesta = new Respuesta<bool>();

            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad is null)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "La categoría que intenta eliminar no existe.";
                respuesta.Codigo = 404;
                return respuesta;
            }

            // Regla de negocio: no se puede eliminar una categoría con productos asociados.
            var productosAsociados = await _repositorioProductos.BuscarAsync(p => p.FkCategoria == id);
            if (productosAsociados.Any())
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "No se puede eliminar la categoría porque tiene productos asociados.";
                respuesta.Codigo = 409;
                return respuesta;
            }

            await _repositorio.EliminarAsync(id);
            await _repositorio.SaveChangesAsync();

            respuesta.Dato = true;
            respuesta.Mensaje = "Categoría eliminada correctamente.";
            return respuesta;
        }
    }
}
