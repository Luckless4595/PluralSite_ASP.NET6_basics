using CityInfo.API.Models.POI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

#pragma warning disable CS8602
namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/poi")]
    public class PointsOfInterestController : ControllerBase 
    {
        private readonly ILogger<PointsOfInterestController> logger;

        public PointsOfInterestController(ILogger<PointsOfInterestController> loggerIn)
        {
            this.logger = loggerIn ?? throw new ArgumentNullException(nameof(loggerIn));
        }

        // GET /api/cities/{cityId}/poi
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPOIs(int cityId)
        {
            
            try {

                // throw new Exception("test");        
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    this.logger.LogInformation($"City with ID {cityId} was not found");
                    return NotFound();
                }
                else
                {
                    this.logger.LogInformation($"Retrieved points of interest for city {city.Name}");
                    return Ok(city.POIs);
                }
        
             } catch (Exception e){
                this.logger.LogCritical("Exception occured",e);
                return StatusCode(500, "An internal error occurred");
            }
        }

        // GET /api/cities/{cityId}/poi/{poiId}
        [HttpGet("{poiId}", Name = "GetPOI")]
        public ActionResult<PointOfInterestDto> GetPOI(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            var targetPOI = city?.POIs.FirstOrDefault(p => p.Id == poiId);

            if (targetPOI == null)
            {
                this.logger.LogInformation($"Point of interest with ID {poiId} was not found in city {city.Name}");
                return NotFound();
            }
            else
            {
                this.logger.LogInformation($"Retrieved point of interest {targetPOI.Name} in city {city.Name}");
                return Ok(targetPOI);
            }
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePOI(int cityId, [FromBody] CreatePointOfInterestDto newPOI)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                this.logger.LogInformation($"City with ID {cityId} was not found");
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

            this.logger.LogInformation($"Added new point of interest {newPOI.Name} to city {city.Name}");

            return CreatedAtRoute("GetPOI", new { cityId = cityId, poiId = newPOIId }, processedPOI);
        }

        [HttpPut("{poiId}")]
        public ActionResult FullyUpdatePOI(int cityId, int poiId, UpdatePOIDto inputPOI)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            var targetPOI = city?.POIs.FirstOrDefault(p => p.Id == poiId);

            if (targetPOI == null)
            {
                this.logger.LogInformation($"Point of interest with ID {poiId} was not found in city {city.Name}");
                return NotFound();
            }

            targetPOI.Name = inputPOI.Name;
            targetPOI.Description = inputPOI.Description;

            this.logger.LogInformation($"Updated point of interest {targetPOI.Name} in city {city.Name}");


        return Ok(targetPOI);
    }

    [HttpPatch("{poiId}")]
    public ActionResult PartiallyUpdatePOI(int cityId, int poiId, JsonPatchDocument<UpdatePOIDto> poiPatch)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        var targetPOI = city?.POIs.FirstOrDefault(p => p.Id == poiId);

        if (targetPOI == null)
        {
            this.logger.LogInformation($"Point of interest with ID {poiId} was not found in city {city.Name}");
            return NotFound();
        }

        var patchedPOI = new UpdatePOIDto()
        {
            Name = targetPOI.Name,
            Description = targetPOI.Description
        };

        poiPatch.ApplyTo(patchedPOI);

        if (!TryValidateModel(patchedPOI))
        {
            this.logger.LogInformation($"Failed to update point of interest {targetPOI.Name} in city {city.Name}");
            return BadRequest(ModelState);
        }

        targetPOI.Name = patchedPOI.Name;
        targetPOI.Description = patchedPOI.Description;

        this.logger.LogInformation($"Updated point of interest {targetPOI.Name} in city {city.Name}");

        return Ok(targetPOI);
    }

    [HttpDelete("{poiId}")]
    public ActionResult DeletePOI(int cityId, int poiId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        var targetPOI = city?.POIs.FirstOrDefault(p => p.Id == poiId);

        if (targetPOI == null)
        {
            this.logger.LogInformation($"Point of interest with ID {poiId} was not found in city {city.Name}");
            return NotFound();
        }

        city.POIs.Remove(targetPOI);

        this.logger.LogInformation($"Deleted point of interest {targetPOI.Name} in city {city.Name}");

        return NoContent();
    }

}   

}
#pragma warning restore CS8602