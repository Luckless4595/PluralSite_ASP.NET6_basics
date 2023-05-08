using CityInfo.API.Models.City;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase {
        
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities(){
            
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id){

            var findCity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id ==id); 

            if (findCity == null){
                return NotFound();
            }
            else{
                return Ok(findCity);
            }
        }
    }
}