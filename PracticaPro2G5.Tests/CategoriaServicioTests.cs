using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.BLL.Servicios.Categoria;
using PracticaPro2G5.DAL.Repositorios.Generico;
using CategoriaEntidad = PracticaPro2G5.DAL.Entidades.Categoria;
using ProductoEntidad = PracticaPro2G5.DAL.Entidades.Producto;

namespace PracticaPro2G5.Tests
{
    /// <summary>
    /// Pruebas unitarias de las reglas de negocio de CategoriaServicio. Se mockean el
    /// repositorio genérico y AutoMapper para aislar la lógica del acceso a datos real.
    /// </summary>
    public class CategoriaServicioTests
    {
        private readonly Mock<IRepositorioGenerico<CategoriaEntidad>> _repoCategorias;
        private readonly Mock<IRepositorioGenerico<ProductoEntidad>> _repoProductos;
        private readonly Mock<IMapper> _mapper;
        private readonly CategoriaServicio _servicio;

        public CategoriaServicioTests()
        {
            _repoCategorias = new Mock<IRepositorioGenerico<CategoriaEntidad>>();
            _repoProductos = new Mock<IRepositorioGenerico<ProductoEntidad>>();
            _mapper = new Mock<IMapper>();
            _servicio = new CategoriaServicio(_repoCategorias.Object, _repoProductos.Object, _mapper.Object);
        }

        [Fact]
        public async Task CrearAsync_CuandoNombreDuplicado_RetornaError409()
        {
            // Arrange: ya existe una categoría con ese nombre.
            _repoCategorias
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<CategoriaEntidad, bool>>>(),
                                          It.IsAny<bool>(),
                                          It.IsAny<Expression<Func<CategoriaEntidad, object>>[]>()))
                .ReturnsAsync(new List<CategoriaEntidad> { new() { Id = 1, Nombre = "Material de curación" } });

            var dto = new CategoriaDto { Nombre = "Material de curación" };

            // Act
            var resultado = await _servicio.CrearAsync(dto);

            // Assert
            Assert.False(resultado.EsCorrecto);
            Assert.Equal(409, resultado.Codigo);
            _repoCategorias.Verify(r => r.AgregarAsync(It.IsAny<CategoriaEntidad>()), Times.Never);
            _repoCategorias.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CrearAsync_CuandoDatosValidos_CreaYRetorna201()
        {
            // Arrange: no hay duplicados.
            _repoCategorias
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<CategoriaEntidad, bool>>>(),
                                          It.IsAny<bool>(),
                                          It.IsAny<Expression<Func<CategoriaEntidad, object>>[]>()))
                .ReturnsAsync(new List<CategoriaEntidad>());

            var dto = new CategoriaDto { Nombre = "Ortopedia" };
            var entidad = new CategoriaEntidad { Id = 5, Nombre = "Ortopedia" };

            _mapper.Setup(m => m.Map<CategoriaEntidad>(dto)).Returns(entidad);
            _mapper.Setup(m => m.Map<CategoriaDto>(entidad)).Returns(new CategoriaDto { Id = 5, Nombre = "Ortopedia" });

            // Act
            var resultado = await _servicio.CrearAsync(dto);

            // Assert
            Assert.True(resultado.EsCorrecto);
            Assert.Equal(201, resultado.Codigo);
            _repoCategorias.Verify(r => r.AgregarAsync(entidad), Times.Once);
            _repoCategorias.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EliminarAsync_CuandoTieneProductosAsociados_RetornaError409()
        {
            // Arrange: la categoría existe pero tiene productos asociados.
            _repoCategorias.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new CategoriaEntidad { Id = 1, Nombre = "Instrumental" });

            _repoProductos
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<ProductoEntidad, bool>>>(),
                                          It.IsAny<bool>(),
                                          It.IsAny<Expression<Func<ProductoEntidad, object>>[]>()))
                .ReturnsAsync(new List<ProductoEntidad> { new() { Id = 10, Nombre = "Bisturí", FkCategoria = 1 } });

            // Act
            var resultado = await _servicio.EliminarAsync(1);

            // Assert
            Assert.False(resultado.EsCorrecto);
            Assert.Equal(409, resultado.Codigo);
            _repoCategorias.Verify(r => r.EliminarAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task EliminarAsync_CuandoNoTieneProductos_EliminaCorrectamente()
        {
            // Arrange: la categoría existe y no tiene productos.
            _repoCategorias.Setup(r => r.ObtenerPorIdAsync(It.IsAny<int>()))
                           .ReturnsAsync(new CategoriaEntidad { Id = 2, Nombre = "Equipo" });

            _repoProductos
                .Setup(r => r.BuscarAsync(It.IsAny<Expression<Func<ProductoEntidad, bool>>>(),
                                          It.IsAny<bool>(),
                                          It.IsAny<Expression<Func<ProductoEntidad, object>>[]>()))
                .ReturnsAsync(new List<ProductoEntidad>());

            // Act
            var resultado = await _servicio.EliminarAsync(2);

            // Assert
            Assert.True(resultado.EsCorrecto);
            Assert.True(resultado.Dato);
            _repoCategorias.Verify(r => r.EliminarAsync(2), Times.Once);
            _repoCategorias.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
