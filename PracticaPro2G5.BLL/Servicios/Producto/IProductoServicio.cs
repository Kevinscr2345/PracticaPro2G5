using PracticaPro2G5.BLL.Dtos;

namespace PracticaPro2G5.BLL.Servicios.Producto
{
    public interface IProductoServicio
    {
        Task<Respuesta<IEnumerable<ProductoDto>>> ObtenerTodosAsync();

        Task<Respuesta<ProductoDto>> ObtenerPorIdAsync(int id);

        Task<Respuesta<ProductoDto>> CrearAsync(ProductoDto dto);

        Task<Respuesta<ProductoDto>> ActualizarAsync(ProductoDto dto);

        Task<Respuesta<bool>> EliminarAsync(int id);
    }
}
