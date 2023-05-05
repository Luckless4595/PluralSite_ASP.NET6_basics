using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/poi")]
    public class PointsOfInterestController : ControllerBase 
    {
        // GET /api/cities/{cityId}/poi
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPOIs(int cityId)
        {
            // Find the city by ID
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 

            // If the city is not found, return 404 Not Found
            if (city == null)
            {
                return NotFound();
            }
            else
            {
                // Return the list of POIs for the city
                return Ok(city.POIs);
            }
        }

        // GET /api/cities/{cityId}/poi/{poiId}
        [HttpGet("{poiId}")]
        public ActionResult<PointOfInterestDto> GetPOI(int cityId, int poiId)
        {
            // Find the city by ID
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 

            // If the city is not found, return 404 Not Found
            if (city == null)
            {
                return NotFound();
            }
            else
            {
                // Find the POI by ID in the city's list of POIs
                var poi = city.POIs.FirstOrDefault(p => p.Id == poiId);

                // If the POI is not found, return 404 Not Found
                if (poi == null)
                {
                    return NotFound();
                }

                // Return the POI
                return Ok(poi);
            }
        }
    }
}
