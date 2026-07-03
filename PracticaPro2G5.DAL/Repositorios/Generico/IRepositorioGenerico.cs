using System.Linq.Expressions;

namespace PracticaPro2G5.DAL.Repositorios.Generico
{
    /// <summary>
    /// Contrato del repositorio genérico. Una sola implementación sirve para CUALQUIER
    /// entidad, evitando escribir un repositorio por cada tabla (requisito del enunciado).
    /// </summary>
    public interface IRepositorioGenerico<T> where T : class
    {
        Task<IEnumerable<T>> ObtenerTodosAsync(bool asNoTracking = true, params Expression<Func<T, object>>[] includes);

        Task<T?> ObtenerPorIdAsync(int id);

        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, bool asNoTracking = true, params Expression<Func<T, object>>[] includes);

        Task AgregarAsync(T entidad);

        Task ActualizarAsync(T entidad);

        Task EliminarAsync(int id);

        Task<int> SaveChangesAsync();
    }
}
