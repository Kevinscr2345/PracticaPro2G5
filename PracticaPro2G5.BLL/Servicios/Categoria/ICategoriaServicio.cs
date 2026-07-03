using PracticaPro2G5.BLL.Dtos;

namespace PracticaPro2G5.BLL.Servicios.Categoria
{
    public interface ICategoriaServicio
    {
        Task<Respuesta<IEnumerable<CategoriaDto>>> ObtenerTodasAsync();

        Task<Respuesta<CategoriaDto>> ObtenerPorIdAsync(int id);

        Task<Respuesta<CategoriaDto>> CrearAsync(CategoriaDto dto);

        Task<Respuesta<CategoriaDto>> ActualizarAsync(CategoriaDto dto);

        Task<Respuesta<bool>> EliminarAsync(int id);
    }
}
