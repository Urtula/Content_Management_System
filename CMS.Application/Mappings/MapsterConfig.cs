using CMS.Application.DTO;
using CMS.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Variant, VariantDTO>.NewConfig();

            TypeAdapterConfig<Category, CategoryDTO>.NewConfig()
                .Map(dest => dest.Contents, src => src.Contents.Adapt<ICollection<ContentDTO>>());

            TypeAdapterConfig<CategoryDTO, Category>.NewConfig()
                .Map(dest => dest.Contents, src => src.Contents.Adapt<ICollection<Content>>());

            TypeAdapterConfig<User, UserDTO>.NewConfig();
        }
    }
}
