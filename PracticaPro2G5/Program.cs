using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using PracticaPro2G5.BLL;
using PracticaPro2G5.BLL.Servicios.Categoria;
using PracticaPro2G5.BLL.Servicios.Producto;
using PracticaPro2G5.DAL.Data;
using PracticaPro2G5.DAL.Repositorios.Generico;
using PracticaPro2G5.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Composición raíz: registro de servicios (Inyección de Dependencias).
// ---------------------------------------------------------------------------

// Capa de presentación (MVC).
builder.Services.AddControllersWithViews();

// Capa de acceso a datos: DbContext con SQLite (cadena de conexión en appsettings.json).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorio genérico abierto: una sola línea lo habilita para cualquier entidad.
builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));

// Servicios de la capa de negocio.
builder.Services.AddScoped<ICategoriaServicio, CategoriaServicio>();
builder.Services.AddScoped<IProductoServicio, ProductoServicio>();

// AutoMapper: perfil de mapeo Entidad <-> DTO.
builder.Services.AddAutoMapper(cfg => { }, typeof(MapeoClases));

var app = builder.Build();

// ---------------------------------------------------------------------------
// Configuración de la tubería HTTP.
// ---------------------------------------------------------------------------

// Manejador global de excepciones: se registra primero para envolver toda la tubería.
app.UseMiddleware<MiddlewareGlobalExceptionHandler>();

// Cultura invariante: los números decimales (precio) usan siempre el punto como separador,
// sin importar la configuración regional del equipo donde se ejecute la aplicación.
var culturaInvariante = new[] { CultureInfo.InvariantCulture };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
    SupportedCultures = culturaInvariante,
    SupportedUICultures = culturaInvariante
});

// Aplica las migraciones y crea la base de datos SQLite en el primer arranque.
using (var scope = app.Services.CreateScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Sirve los archivos estáticos de wwwroot (CSS, JS, librerías) con su MIME correcto.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
