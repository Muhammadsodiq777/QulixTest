using AutoMapper;
using QulixTest.Core.Domain;
using QulixTest.Core.Model;

namespace QulixTest.Persistence.MapperConfigurations
{
    public class MapperInitilizer : Profile
    {
        public MapperInitilizer()
        {
            CreateMap<Author, UserDTO>().ReverseMap();
            CreateMap<Tag, TagDTO>().ReverseMap();
            CreateMap<Text, TextDTO>().ReverseMap();
        }

    }
}
