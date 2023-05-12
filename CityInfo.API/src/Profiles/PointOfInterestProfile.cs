using AutoMapper;

using CityInfo.API.src.Entities;
using CityInfo.API.src.Models.POI;

namespace CityInfo.API.src.Profiles{
    public class PointOfInterestProfile : Profile{

        public PointOfInterestProfile(){
            CreateMap<PointOfInterest, PointOfInterestDto>();
            CreateMap<CreatePointOfInterestDto, PointOfInterest>();
            CreateMap<UpdatePointOfInterestDto, PointOfInterest>();
            CreateMap<PointOfInterest, UpdatePointOfInterestDto>();
        }

    }
}