using CityInfo.API.Models.City;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase {

       private readonly CitiesDataStore citiesDataStore;

        public CitiesController(CitiesDataStore citiesDataStoreIn)
        {
            this.citiesDataStore = citiesDataStoreIn ?? 
            throw new ArgumentNullException(nameof(citiesDataStoreIn));
        }


        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities(){
            
            return Ok(this.citiesDataStore.Cities);
        }

        [HttpGet("{id}")]
        public ActionResult<CityDto> GetCity(int id){

            var findCity = this.citiesDataStore.Cities.FirstOrDefault(c => c.Id ==id); 

            if (findCity == null){
                return NotFound();
            }
            else{
                return Ok(findCity);
            }
        }
    }
}