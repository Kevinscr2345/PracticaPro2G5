using AutoMapper;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.DAL.Repositorios.Generico;
// Alias para distinguir las ENTIDADES (EF Core) de los namespaces y DTOs homónimos.
using CategoriaEntidad = PracticaPro2G5.DAL.Entidades.Categoria;
using ProductoEntidad = PracticaPro2G5.DAL.Entidades.Producto;

namespace PracticaPro2G5.BLL.Servicios.Producto
{
    /// <summary>
    /// Lógica de negocio de Productos. Valida la existencia de la categoría relacionada y
    /// las reglas de precio, stock y unicidad antes de tocar el repositorio.
    /// </summary>
    public class ProductoServicio : IProductoServicio
    {
        private readonly IRepositorioGenerico<ProductoEntidad> _repositorio;
        private readonly IRepositorioGenerico<CategoriaEntidad> _repositorioCategorias;
        private readonly IMapper _mapper;

        public ProductoServicio(
            IRepositorioGenerico<ProductoEntidad> repositorio,
            IRepositorioGenerico<CategoriaEntidad> repositorioCategorias,
            IMapper mapper)
        {
            _repositorio = repositorio;
            _repositorioCategorias = repositorioCategorias;
            _mapper = mapper;
        }

        public async Task<Respuesta<IEnumerable<ProductoDto>>> ObtenerTodosAsync()
        {
            var respuesta = new Respuesta<IEnumerable<ProductoDto>>();

            // Include de la categoría para poder mostrar su nombre en la tabla.
            var productos = await _repositorio.ObtenerTodosAsync(true, p => p.Categoria!);
            respuesta.Dato = _mapper.Map<IEnumerable<ProductoDto>>(productos);

            return respuesta;
        }

        public async Task<Respuesta<ProductoDto>> ObtenerPorIdAsync(int id)
        {
            var respuesta = new Respuesta<ProductoDto>();

            var producto = await _repositorio.ObtenerPorIdAsync(id);
            if (producto is null)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "El producto solicitado no existe.";
                respuesta.Codigo = 404;
                return respuesta;
            }

            respuesta.Dato = _mapper.Map<ProductoDto>(producto);
            return respuesta;
        }

        public async Task<Respuesta<ProductoDto>> CrearAsync(ProductoDto dto)
        {
            var respuesta = new Respuesta<ProductoDto>();

            var validacion = await ValidarReglasAsync(dto);
            if (validacion is not null)
            {
                return validacion;
            }

            var entidad = _mapper.Map<ProductoEntidad>(dto);
            await _repositorio.AgregarAsync(entidad);
            await _repositorio.SaveChangesAsync();

            respuesta.Dato = _mapper.Map<ProductoDto>(entidad);
            respuesta.Mensaje = "Producto creado correctamente.";
            respuesta.Codigo = 201;
            return respuesta;
        }

        public async Task<Respuesta<ProductoDto>> ActualizarAsync(ProductoDto dto)
        {
            var respuesta = new Respuesta<ProductoDto>();

            if (dto.Id is null or <= 0)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "El identificador del producto no es válido.";
                respuesta.Codigo = 400;
                return respuesta;
            }

            var entidad = await _repositorio.ObtenerPorIdAsync(dto.Id.Value);
            if (entidad is null)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "El producto que intenta actualizar no existe.";
                respuesta.Codigo = 404;
                return respuesta;
            }

            var validacion = await ValidarReglasAsync(dto);
            if (validacion is not null)
            {
                return validacion;
            }

            entidad.Nombre = dto.Nombre;
            entidad.Descripcion = dto.Descripcion;
            entidad.Precio = dto.Precio;
            entidad.Stock = dto.Stock;
            entidad.FkCategoria = dto.FkCategoria;

            await _repositorio.ActualizarAsync(entidad);
            await _repositorio.SaveChangesAsync();

            respuesta.Dato = _mapper.Map<ProductoDto>(entidad);
            respuesta.Mensaje = "Producto actualizado correctamente.";
            return respuesta;
        }

        public async Task<Respuesta<bool>> EliminarAsync(int id)
        {
            var respuesta = new Respuesta<bool>();

            var entidad = await _repositorio.ObtenerPorIdAsync(id);
            if (entidad is null)
            {
                respuesta.EsCorrecto = false;
                respuesta.Mensaje = "El producto que intenta eliminar no existe.";
                respuesta.Codigo = 404;
                return respuesta;
            }

            await _repositorio.EliminarAsync(id);
            await _repositorio.SaveChangesAsync();

            respuesta.Dato = true;
            respuesta.Mensaje = "Producto eliminado correctamente.";
            return respuesta;
        }

        /// <summary>
        /// Reglas de negocio compartidas entre crear y actualizar. Devuelve una respuesta de
        /// error si alguna regla se incumple, o <c>null</c> si todo es válido.
        /// </summary>
        private async Task<Respuesta<ProductoDto>?> ValidarReglasAsync(ProductoDto dto)
        {
            // Regla de negocio: la categoría seleccionada debe existir.
            var categoria = await _repositorioCategorias.ObtenerPorIdAsync(dto.FkCategoria);
            if (categoria is null)
            {
                return new Respuesta<ProductoDto>
                {
                    EsCorrecto = false,
                    Mensaje = "La categoría seleccionada no existe.",
                    Codigo = 404
                };
            }

            // Regla de negocio: el precio debe ser mayor a cero.
            if (dto.Precio <= 0)
            {
                return new Respuesta<ProductoDto>
                {
                    EsCorrecto = false,
                    Mensaje = "El precio del producto debe ser mayor a cero.",
                    Codigo = 400
                };
            }

            // Regla de negocio: el stock no puede ser negativo.
            if (dto.Stock < 0)
            {
                return new Respuesta<ProductoDto>
                {
                    EsCorrecto = false,
                    Mensaje = "El stock del producto no puede ser negativo.",
                    Codigo = 400
                };
            }

            // Regla de negocio: no se permite otro producto con el mismo nombre en la misma categoría.
            var idActual = dto.Id ?? 0;
            var duplicados = await _repositorio.BuscarAsync(
                p => p.FkCategoria == dto.FkCategoria
                     && p.Nombre.ToLower() == dto.Nombre.ToLower()
                     && p.Id != idActual);
            if (duplicados.Any())
            {
                return new Respuesta<ProductoDto>
                {
                    EsCorrecto = false,
                    Mensaje = $"Ya existe un producto llamado '{dto.Nombre}' en esta categoría.",
                    Codigo = 409
                };
            }

            return null;
        }
    }
}
