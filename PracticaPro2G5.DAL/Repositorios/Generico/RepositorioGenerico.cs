using Microsoft.EntityFrameworkCore;
using PracticaPro2G5.DAL.Data;
using System.Linq.Expressions;

namespace PracticaPro2G5.DAL.Repositorios.Generico
{
    /// <summary>
    /// Implementación genérica del repositorio. Los métodos de escritura solo modifican el
    /// ChangeTracker; el guardado real ocurre al llamar <see cref="SaveChangesAsync"/>
    /// (patrón Unit of Work implícito respaldado por el propio DbContext).
    /// </summary>
    public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : class
    {
        private readonly AppDbContext _contexto;
        private readonly DbSet<T> _dbSet;

        public RepositorioGenerico(AppDbContext contexto)
        {
            _contexto = contexto;
            _dbSet = _contexto.Set<T>();
        }

        public async Task<IEnumerable<T>> ObtenerTodosAsync(bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> consulta = _dbSet;

            foreach (var include in includes)
            {
                consulta = consulta.Include(include);
            }

            if (asNoTracking)
            {
                consulta = consulta.AsNoTracking();
            }

            return await consulta.ToListAsync();
        }

        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> consulta = _dbSet;

            foreach (var include in includes)
            {
                consulta = consulta.Include(include);
            }

            if (asNoTracking)
            {
                consulta = consulta.AsNoTracking();
            }

            return await consulta.Where(predicado).ToListAsync();
        }

        public async Task AgregarAsync(T entidad)
        {
            await _dbSet.AddAsync(entidad);
        }

        public Task ActualizarAsync(T entidad)
        {
            _dbSet.Update(entidad);
            return Task.CompletedTask;
        }

        public async Task EliminarAsync(int id)
        {
            var entidad = await _dbSet.FindAsync(id);

            if (entidad is not null)
            {
                _dbSet.Remove(entidad);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _contexto.SaveChangesAsync();
        }
    }
}
