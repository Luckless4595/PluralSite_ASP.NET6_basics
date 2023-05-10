using AutoMapper;

using CityInfo.API.Entities;
using CityInfo.API.Models.City;

namespace CityInfo.API.Profiles{
    public class CityProfile : Profile{

        public CityProfile(){
            CreateMap<City,CityWithoutPOIDto>();
            CreateMap<City,CityDto>();
        }

    }
}