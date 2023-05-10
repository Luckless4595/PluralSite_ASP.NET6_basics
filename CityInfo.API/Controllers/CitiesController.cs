using CityInfo.API.Models.City;
using CityInfo.API.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase {

        private readonly ICityInfoRepository cityInfoRepository;
        public CitiesController(ICityInfoRepository citiesDataStoreIn)
        {
            this.cityInfoRepository = citiesDataStoreIn ?? 
            throw new ArgumentNullException(nameof(citiesDataStoreIn));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDto>>> GetCities(){
            
            var cityEntities = await this.cityInfoRepository.GetCitiesAsync();
            var output = new List<CityWithoutPOIDto>();

            foreach (var cityEntity in cityEntities){
                output.Add( 
                    new CityWithoutPOIDto{
                        Id = cityEntity.Id,
                        Name = cityEntity.Name,
                        Description = cityEntity.Description
                    }
                );
            }

            return Ok(output);
        }

       [HttpGet("{id}")]
        public async Task<ActionResult<CityWithoutPOIDto>> GetCity(int cityId)
        {
            var cityEntity = await this.cityInfoRepository.GetCityAsync(cityId, false);

            if (cityEntity == null)
            {
                return NotFound();
            }

            var output = new CityWithoutPOIDto
            {
                Id = cityEntity.Id,
                Name = cityEntity.Name,
                Description = cityEntity.Description
            };

            return Ok(output);
        }

    }
}