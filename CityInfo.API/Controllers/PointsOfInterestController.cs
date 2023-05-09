using CityInfo.API.Models.POI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            return city == null ? NotFound() : Ok(city.POIs);
        }

        // GET /api/cities/{cityId}/poi/{poiId}
        [HttpGet("{poiId}", Name = "GetPOI")]
        public ActionResult<PointOfInterestDto> GetPOI(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 
            var poi = city?.POIs.FirstOrDefault(p => p.Id == poiId);
            return poi == null ? NotFound() : Ok(poi);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePOI(int cityId, [FromBody] CreatePointOfInterestDto newPOI)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 
            if (city == null) return NotFound();

            var newPOIId = CitiesDataStore.Current.Cities.SelectMany(c => c.POIs).Max(p => p.Id) + 1;

            var processedPOI = new PointOfInterestDto()
            {
                Id = newPOIId,
                Name = newPOI.Name,
                Description = newPOI.Description
            };

            city.POIs.Add(processedPOI);
        
            return CreatedAtRoute("GetPOI", new { cityId = cityId, poiId = newPOIId }, processedPOI);
        }

        [HttpPut("{poiId}")]
        public ActionResult FullyUpdatePOI(int cityId, int poiId, UpdatePOIDto inputPOI)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId); 
            var targetPOI = city?.POIs.FirstOrDefault(p => p.Id == poiId);
            if (targetPOI == null) return NotFound();
        
            targetPOI.Name = inputPOI.Name;
            targetPOI.Description = inputPOI.Description;
        
            return Ok(targetPOI);
        }

        [HttpPatch("{poiId}")]
        public ActionResult PartiallyUpdatePOI(int cityId, int poiId, JsonPatchDocument<UpdatePOIDto> poiPatch)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            var targetPOI = city?.POIs.FirstOrDefault(p => p.Id == poiId);
            if (targetPOI == null) return NotFound();

            var patchedPOI = new UpdatePOIDto()
            {
                Name = targetPOI.Name,
                Description = targetPOI.Description
            };

            poiPatch.ApplyTo(patchedPOI);
    
            return !TryValidateModel(patchedPOI) ? BadRequest(ModelState) : Ok(targetPOI);
        }
    }
}
