using AutoMapper;

using CityInfo.API.src.Entities;
using CityInfo.API.src.Models.City;

namespace CityInfo.API.src.Profiles{
    public class CityProfile : Profile{

        public CityProfile(){
            CreateMap<City,CityWithoutPOIDto>();
            CreateMap<City,CityDto>();
        }

    }
}