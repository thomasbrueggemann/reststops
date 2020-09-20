
using System;
using AutoMapper;
using Microsoft.Azure.Cosmos.Spatial;
using MongoDB.Driver.GeoJsonObjectModel;
using Reststops.Core.Entities;
using Reststops.Core.Enums;

namespace Reststops.Infrastructure.Data.DAO
{
    public class EntityDAOMappingProfile : Profile
    {
        public EntityDAOMappingProfile()
        {
            CreateMap<Reststop, ReststopDAO>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID.ToString()))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(src.Longitude, src.Latitude))))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.GetName(typeof(ReststopType), src.Type)))
                .ReverseMap()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => Convert.ToUInt64(src.ID)))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Coordinates.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Location.Coordinates.Longitude))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse(typeof(ReststopType), src.Type)));
        }
    }
}
