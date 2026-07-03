using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PracticaPro2G5.DAL.Data
{
    /// <summary>
    /// Fábrica usada ÚNICAMENTE por las herramientas de EF Core (dotnet ef) en tiempo de
    /// diseño, para generar migraciones sin necesidad de arrancar la capa de presentación.
    /// En ejecución real la cadena de conexión proviene de appsettings.json.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var opciones = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=PracticaPro2G5.db")
                .Options;

            return new AppDbContext(opciones);
        }
    }
}
