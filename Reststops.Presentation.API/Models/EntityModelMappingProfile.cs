using System;
using AutoMapper;
using Reststops.Core.Entities;
using Reststops.Core.Enums;

namespace Reststops.Presentation.API.Models
{
    public class EntityModelMappingProfile : Profile
    {
        public EntityModelMappingProfile()
        {
            CreateMap<Reststop, ReststopModel>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.GetName(typeof(ReststopType), src.Type)));

            CreateMap<ForwardGeocodingFeature, PlaceModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.PlaceName))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Center[1]))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Center[0]));
        }
    }
}
