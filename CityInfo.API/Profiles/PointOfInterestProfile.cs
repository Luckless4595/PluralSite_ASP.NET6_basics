using AutoMapper;

using CityInfo.API.Entities;
using CityInfo.API.Models.POI;

namespace CityInfo.API.Profiles{
    public class PointOfInterestProfile : Profile{

        public PointOfInterestProfile(){
            CreateMap<PointOfInterest, PointOfInterestDto>();
            CreateMap<CreatePointOfInterestDto, PointOfInterest>();
        }

    }
}