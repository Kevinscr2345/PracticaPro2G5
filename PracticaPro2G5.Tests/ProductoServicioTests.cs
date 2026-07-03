using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.BLL.Servicios.Producto;
using PracticaPro2G5.DAL.Repositorios.Generico;
using CategoriaEntidad = PracticaPro2G5.DAL.Entidades.Categoria;
using ProductoEntidad = PracticaPro2G5.DAL.Entidades.Producto;

namespace PracticaPro2G5.Tests
{
    /// <summary>
    /// Pruebas unitarias de las reglas de negocio de ProductoServicio.
    /// </summary>
    public class ProductoServicioTests
    {
        private readonly Mock<IRepositorioGenerico<ProductoEntidad>> _repoProductos;
        private readonly Mock<IRepositorioGenerico<CategoriaEntidad>> _repoCategorias;
        private readonly Mock<IMapper> _mapper;
        private readonly ProductoServicio _servicio;

        public ProductoServicioTests()
        {
            _repoProductos = new Mock<IRepositorioGenerico<ProductoEntidad>>();
            _repoCategorias = new Mock<IRepositorioGenerico<CategoriaEntidad>>();
            _mapper = new Mock<IMapper>();
            _servicio = new ProductoServicio(_repoProductos.Object, _repoCategorias.Object, _mapper.Object);
        }

        [Fact]
        public async Task CrearAsync_CuandoCategoriaNoExiste_RetornaError404()
        {
            // Arrange: la categoría seleccionada no existe.
            _repoCategorias.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                           .ReturnsAsync((CategoriaEntidad?)null);

            var dto = new ProductoDto { Nombre = "Jeringa", Precio = 500, Stock = 10, FkCategoria = 99 };

            // Act
            var resultado = await _servicio.CrearAsync(dto);

            // Assert
            Assert.False(resultado.EsCorrecto);
            Assert.Equal(404, resultado.Codigo);
            _repoProductos.Verify(r => r.AgregarAsync(It.IsAny<ProductoEntidad>()), Times.Never);
        }

        [Fact]
        public async Task CrearAsync_CuandoPrecioEsCero_RetornaError400()
        {
            // Arrange: la categoría existe, pero el precio es inválido.
            _repoCategorias.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new CategoriaEntidad { Id = 1, Nombre = "Material de curación" });

            var dto = new ProductoDto { Nombre = "Gasa", Precio = 0, Stock = 10, FkCategoria = 1 };

            // Act
            var resultado = await _servicio.CrearAsync(dto);

            // Assert
            Assert.False(resultado.EsCorrecto);
            Assert.Equal(400, resultado.Codigo);
            _repoProductos.Verify(r => r.AgregarAsync(It.IsAny<ProductoEntidad>()), Times.Never);
        }

        [Fact]
        public async Task CrearAsync_CuandoDatosValidos_CreaYRetorna201()
        {
            // Arrange: categoría existe, precio y stock válidos, sin duplicados.
            _repoCategorias.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new CategoriaEntidad { Id = 1, Nombre = "Material de curación" });

            _repoProductos
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<ProductoEntidad, bool>>>(),
                                          It.IsAny<bool>(),
                                          It.IsAny<Expression<Func<ProductoEntidad, object>>[]>()))
                .ReturnsAsync(new List<ProductoEntidad>());

            var dto = new ProductoDto { Nombre = "Gasa estéril", Precio = 350, Stock = 100, FkCategoria = 1 };
            var entidad = new ProductoEntidad { Id = 7, Nombre = "Gasa estéril", Precio = 350, Stock = 100, FkCategoria = 1 };

            _mapper.Setup(m => m.Map<ProductoEntidad>(dto)).Returns(entidad);
            _mapper.Setup(m => m.Map<ProductoDto>(entidad)).Returns(new ProductoDto { Id = 7, Nombre = "Gasa estéril" });

            // Act
            var resultado = await _servicio.CrearAsync(dto);

            // Assert
            Assert.True(resultado.EsCorrecto);
            Assert.Equal(201, resultado.Codigo);
            _repoProductos.Verify(r => r.AgregarAsync(entidad), Times.Once);
            _repoProductos.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
