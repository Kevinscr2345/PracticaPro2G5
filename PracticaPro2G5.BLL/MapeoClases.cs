using AutoMapper;
using PracticaPro2G5.BLL.Dtos;
using PracticaPro2G5.DAL.Entidades;

namespace PracticaPro2G5.BLL
{
    /// <summary>
    /// Perfil único de AutoMapper. Centraliza toda la conversión Entidad &lt;-&gt; DTO.
    /// </summary>
    public class MapeoClases : Profile
    {
        public MapeoClases()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();

            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.NombreCategoria,
                           opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
                .ReverseMap()
                // Al mapear DTO -> Entidad se ignora la navegación (solo se usa la FK).
                .ForMember(dest => dest.Categoria, opt => opt.Ignore());
        }
    }
}
