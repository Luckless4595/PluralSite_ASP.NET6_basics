using CityInfo.API.Models.POI;
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
        [HttpGet("{poiId}", Name = "GetPOI")]
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

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePOI(int cityId, [FromBody] CreatePointOfInterestDto newPOI)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 

            // If the city is not found, return 404 Not Found
            if (city == null)
            {
                return NotFound();
            }

            var newPOIId = CitiesDataStore.Current.Cities.SelectMany(c => c.POIs).Max(p => p.Id) + 1;

            var processedPOI = new PointOfInterestDto()
            {
                Id = newPOIId,
                Name = newPOI.Name,
                Description = newPOI.Description
            };

            city.POIs.Add(processedPOI);
            
            return CreatedAtRoute("GetPOI", 
            new { cityId = cityId, poiId = newPOIId }, processedPOI);
        }

        [HttpPut("{poiId}")]
        public ActionResult UpdatePOI(int cityId, int poiId, UpdatePOIDto inputPOI){

            // Find the city by ID
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 

            // If the city is not found, return 404 Not Found
            if (city == null)
            {
                return NotFound();
            }
                // Find the POI by ID in the city's list of POIs
                var targetPOI= city.POIs.FirstOrDefault(p => p.Id == poiId);

                // If the POI is not found, return 404 Not Found
                if (targetPOI == null)
                {
                    return NotFound();
                }
            
            targetPOI.Name = inputPOI.Name;
            targetPOI.Description = inputPOI.Description;
            
            return Ok(targetPOI);
        }
    }
}
