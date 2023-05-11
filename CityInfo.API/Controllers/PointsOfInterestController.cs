using CityInfo.API.Models.POI;
using CityInfo.API.Services.Interfaces;
using CityInfo.API.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;


#pragma warning disable CS8602
namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/poi")]
    public class PointsOfInterestController : ControllerBase 
    {
        private readonly ILogger<PointsOfInterestController> logger;
        private readonly IMailService mailService;
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> loggerIn,
            IMailService mailServiceIn,
            ICityInfoRepository cityInfoRepositoryIn,
            IMapper mapperIn)
        {
            this.logger = loggerIn ?? 
                throw new ArgumentNullException(nameof(loggerIn));
            
            this.mailService = mailServiceIn ?? 
                throw new ArgumentNullException(nameof(mailServiceIn));

            this.cityInfoRepository = cityInfoRepositoryIn ??
                throw new ArgumentNullException(nameof(cityInfoRepositoryIn));

            this.mapper = mapperIn ??
            throw new ArgumentNullException (nameof(mapperIn));
        }

        // GET /api/cities/{cityId}/poi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPOIs(int cityId)
        {            
            try {

                if(! await this.cityInfoRepository.CheckCityExistsAsync(cityId)){
                    this.logger.LogInformation($"City with Id {cityId} was not found");
                    return NotFound();
                }

                var poiEntitiesInCity = await this.cityInfoRepository.GetCityPOIsAsync(cityId);
                var output = this.mapper.Map<IEnumerable<PointOfInterestDto>>(poiEntitiesInCity);

                this.logger.LogInformation($"Retrieved points of interest for city with ID {cityId}");

                return Ok(output);
            }
            catch (Exception e){
                this.logger.LogCritical("Exception occurred", e);
                return StatusCode(500, "An internal error occurred");
            }
        }

        // GET /api/cities/{cityId}/poi/{poiId}
        [HttpGet("{poiId}", Name = "GetPOI")]
        public async Task<ActionResult<PointOfInterestDto>> GetPOI(int cityId, int poiId)
        {
            try {
                if(! await this.cityInfoRepository.CheckCityExistsAsync(cityId)){
                    this.logger.LogInformation($"City with Id {cityId} was not found");
                    return NotFound();
                }

                var poiEntity = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
                if (poiEntity == null) return NotFound();

                var output = this.mapper.Map<PointOfInterestDto>(poiEntity);

                this.logger.LogInformation($"Retrieved point of interest with ID {poiId} for city with ID {cityId}");

                return Ok(output);
            }
            catch (Exception e){
                this.logger.LogCritical("Exception occurred", e);
                return StatusCode(500, "An internal error occurred");
            }
        }


        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePOI(int cityId, CreatePointOfInterestDto newPOI)
        {
            try {
                var poiToEnterInDB = this.mapper.Map<PointOfInterest>(newPOI);
                Console.WriteLine($"Poi Received Name: {poiToEnterInDB.Name}");

                // check that no duplicate name poi
                var poisInCityResult = await GetPOIs(cityId);
                if (poisInCityResult.Result is OkObjectResult okResult)
                {
                    var poisInCity = okResult.Value as IEnumerable<PointOfInterestDto>;
                    if (poisInCity != null && poisInCity.Any(p => p.Name == poiToEnterInDB.Name))
                        return BadRequest($"A point of interest with the name {poiToEnterInDB.Name} already exists in this city.");
                }
                else
                {
                    return BadRequest("Failed to retrieve points of interest for the specified city.");
                }

                await this.cityInfoRepository.AddPointOfInterestForCityAsync(cityId, poiToEnterInDB);

                await this.cityInfoRepository.SaveChangesAsync();  
                var poiEnteredInDB = this.mapper.Map<PointOfInterestDto>(poiToEnterInDB);

                this.logger.LogInformation($"Added new point of interest {poiEnteredInDB.Name} to city with ID {cityId}");
                return CreatedAtRoute("GetPOI", new { cityId = cityId, poiId = poiEnteredInDB.Id }, poiEnteredInDB);
            }
            catch (Exception e){
                this.logger.LogCritical("Exception occurred", e);
                return StatusCode(500, "An internal error occurred");
            }
        }


        [HttpPut("{poiId}")]
        public async Task<ActionResult> FullyUpdatePOI(
            int cityId, int poiId, UpdatePointOfInterestDto inputPOI)
        {
            try {

            if(! await this.cityInfoRepository.CheckCityExistsAsync(cityId)){
                this.logger.LogInformation($"City with Id {cityId} was not found");
                return NotFound();
            }

            var poiEntity = await this.cityInfoRepository.GetPOIAsync(cityId,poiId);
            if (poiEntity == null) return NotFound();

            // special exception, when calling map like this, the values from
            // the input are automatically written to entity 
            this.mapper.Map(inputPOI, poiEntity);
            await this.cityInfoRepository.SaveChangesAsync();

            this.logger.LogInformation($"Updated point of interest with ID {poiId} in city with ID {cityId}");

            var updatedPOI = this.mapper.Map<PointOfInterestDto>(poiEntity);
            return Ok(updatedPOI);
        }
        catch (Exception e){
            this.logger.LogCritical("Exception occurred", e);
            return StatusCode(500, "An internal error occurred");
        }
    }

    [HttpPatch("{poiId}")]
    public async Task<ActionResult> PartiallyUpdatePOI(
        int cityId, int poiId, JsonPatchDocument<UpdatePointOfInterestDto> poiPatchDocument)
    {
        try {

            if(! await this.cityInfoRepository.CheckCityExistsAsync(cityId)){
                this.logger.LogInformation($"City with Id {cityId} was not found");
                return NotFound();
            }

            var poiEntityToPatch = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
            if (poiEntityToPatch == null) return NotFound();

            var poiDtoToPatch = this.mapper.Map<UpdatePointOfInterestDto>(poiEntityToPatch);
            poiPatchDocument.ApplyTo(poiDtoToPatch, ModelState);

            if (!TryValidateModel(poiDtoToPatch)) return BadRequest(ModelState);

            // again special mapping case that updates the values of the 2nd object from the 1st
            this.mapper.Map(poiDtoToPatch, poiEntityToPatch);
            await this.cityInfoRepository.SaveChangesAsync();
            this.logger.LogInformation($"Partially updated point of interest with ID {poiId} in city with ID {cityId}");

            var patchedPOI = this.mapper.Map<PointOfInterestDto>(poiEntityToPatch);
            return Ok(patchedPOI);
        }
        catch (Exception e){
            this.logger.LogCritical("Exception occurred", e);
            return StatusCode(500, "An internal error occurred");
        }
    }

    [HttpDelete("{poiId}")]
    public async Task<ActionResult> DeletePOI(int cityId, int poiId)
    {
        try {

            if(! await this.cityInfoRepository.CheckCityExistsAsync(cityId)){
                this.logger.LogInformation($"City with Id {cityId} was not found");
                return NotFound();
            }

            var poiEntity = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
            if (poiEntity == null) return NotFound();

            this.cityInfoRepository.DeletePointOfInterestAsync(poiEntity);
            await this.cityInfoRepository.SaveChangesAsync();

            this.mailService.Send("A POI was deleted",
                $"Deleted point of interest {poiEntity.Name} in city with ID {cityId}");

            this.logger.LogInformation($"Deleted point of interest with ID {poiId} in city with ID {cityId}");

            return NoContent();
        }
        catch (Exception e){
            this.logger.LogCritical("Exception occurred", e);
            return StatusCode(500, "An internal error occurred");
        }
    }
}

}
#pragma warning restore CS8602