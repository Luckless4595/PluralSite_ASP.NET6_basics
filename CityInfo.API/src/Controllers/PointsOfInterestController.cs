using CityInfo.API.src.Models.POI;
using CityInfo.API.src.Services.Interfaces;
using CityInfo.API.src.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using AutoMapper;

#pragma warning disable CS8602, CS0168
namespace CityInfo.API.src.Controllers
{
    /// <summary>
    /// Controller for handling points of interest operations.
    /// </summary>
    [ApiController]
    [Authorize]
    [ApiVersion("2.0")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cities/{cityId}/poi")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly IMailService mailService;
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;
        const int maxPOIPageSize = 20;
        private readonly ILogger<PointsOfInterestController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointsOfInterestController"/> class.
        /// </summary>
        /// <param name="mailServiceIn">The mail service.</param>
        /// <param name="cityInfoRepositoryIn">The city info repository.</param>
        /// <param name="mapperIn">The mapper.</param>
        /// <param name="logger">The logger.</param>
        public PointsOfInterestController(
            IMailService mailServiceIn,
            ICityInfoRepository cityInfoRepositoryIn,
            IMapper mapperIn,
            ILogger<PointsOfInterestController> logger
        )
        {
            this.mailService = mailServiceIn ??
                throw new ArgumentNullException(nameof(mailServiceIn));

            this.cityInfoRepository = cityInfoRepositoryIn ??
                throw new ArgumentNullException(nameof(cityInfoRepositoryIn));

            this.mapper = mapperIn ??
            throw new ArgumentNullException(nameof(mapperIn));

            this._logger = logger ??
            throw new ArgumentException(nameof(logger));
        }

        /// <summary>
        /// Gets the points of interest for a city.
        /// </summary>
        /// <param name="cityId">The city ID.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The list of points of interest.</returns>
        [HttpGet]
        // [Authorize(Policy = "CanOnlyRequestPOIofHomeCity")]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPOIs(
            int cityId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("ayo");

                if (pageSize > maxPOIPageSize)
                    pageSize = maxPOIPageSize;

                if (!await this.cityInfoRepository.CheckCityExistsAsync(cityId))
                {
                    return NotFound();
                }

                var (poiEntitiesInCity, pagingMetadata) = await this.cityInfoRepository.GetCityPOIsAsync(cityId, pageNumber, pageSize);
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagingMetadata));

                var output = this.mapper.Map<IEnumerable<PointOfInterestDto>>(poiEntitiesInCity);

                return Ok(output);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An internal error occurred");
            }
        }

        /// <summary>
        /// Gets a specific point of interest for a city.
        /// </summary>
        /// <param name="cityId">The city ID.</param>
        /// <param name="poiId">The point of interest ID.</param>
        /// <returns>The point of interest.</returns>
        [HttpGet("{poiId}", Name = "GetPOI")]
        // [Authorize(Policy = "CanOnlyRequestPOIofHomeCity")]
        public async Task<ActionResult<PointOfInterestDto>> GetPOI(int cityId, int poiId)
        {
            try
            {
                if (!await this.cityInfoRepository.CheckCityExistsAsync(cityId))
                {
                    return NotFound();
                }

                var poiEntity = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
                if (poiEntity == null) return NotFound();

                var output = this.mapper.Map<PointOfInterestDto>(poiEntity);

                return Ok(output);
            }
            catch (Exception e)
            {
                return StatusCode(500, "An internal error occurred");
            }
        }


        /// <summary>
        /// Creates a new point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="newPOI">The data for the new point of interest.</param>
        /// <returns>The created point of interest.</returns>
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePOI(int cityId, CreatePointOfInterestDto newPOI)
        {
        try
        {
        var poiToEnterInDB = this.mapper.Map<PointOfInterest>(newPOI);
        _logger.LogInformation($"Poi Received Name: {poiToEnterInDB.Name}");

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

        return CreatedAtRoute("GetPOI", new { cityId = cityId, poiId = poiEnteredInDB.Id }, poiEnteredInDB);
        }
        catch (Exception e)
        {
        return StatusCode(500, "An internal error occurred");
        }
        }

        /// <summary>
        /// Fully updates a point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="poiId">The ID of the point of interest.</param>
        /// <param name="inputPOI">The updated data for the point of interest.</param>
        /// <returns>The updated point of interest.</returns>
        [HttpPut("{poiId}")]
        public async Task<ActionResult> FullyUpdatePOI(int cityId, int poiId, UpdatePointOfInterestDto inputPOI)
        {
        try
        {
        if (!await this.cityInfoRepository.CheckCityExistsAsync(cityId))
        {
            return NotFound();
        }

        var poiEntity = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
        if (poiEntity == null) return NotFound();

        this.mapper.Map(inputPOI, poiEntity);
        await this.cityInfoRepository.SaveChangesAsync();

        var updatedPOI = this.mapper.Map<PointOfInterestDto>(poiEntity);
        return Ok(updatedPOI);
        }
        catch (Exception e)
        {
        return StatusCode(500, "An internal error occurred");
        }
        }

        /// <summary>
        /// Partially updates a point of interest for a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="poiId">The ID of the point of interest.</param>
        /// <param name="poiPatchDocument">The JSON patch document containing the updates.</param>
        /// <returns>The patched point of interest.</returns>
        [HttpPatch("{poiId}")]
        public async Task<ActionResult> PartiallyUpdatePOI(int cityId, int poiId, JsonPatchDocument<UpdatePointOfInterestDto> poiPatchDocument)
        {
        try
        {
        if (!await this.cityInfoRepository.CheckCityExistsAsync(cityId))
        {
            return NotFound();
        }

        var poiEntityToPatch = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
        if (poiEntityToPatch == null) return NotFound();

        var poiDtoToPatch = this.mapper.Map<UpdatePointOfInterestDto>(poiEntityToPatch);
        poiPatchDocument.ApplyTo(poiDtoToPatch, ModelState);

        if (!TryValidateModel(poiDtoToPatch)) return BadRequest(ModelState);

        this.mapper.Map(poiDtoToPatch, poiEntityToPatch);
        await this.cityInfoRepository.SaveChangesAsync();

        var patchedPOI = this.mapper.Map<PointOfInterestDto>(poiEntityToPatch);
        return Ok(patchedPOI);
        }
        catch (Exception e)
        {
        return StatusCode(500, "An internal error occurred");
        }
        }

        /// <summary>
        /// Deletes a point of interest from a city.
        /// </summary>
        /// <param name="cityId">The ID of the city.</param>
        /// <param name="poiId">The ID of the point of interest to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{poiId}")]
        public async Task<ActionResult> DeletePOI(int cityId, int poiId)
        {
        try
        {
            if (!await this.cityInfoRepository.CheckCityExistsAsync(cityId))
            {
                return NotFound();
            }

            var poiEntity = await this.cityInfoRepository.GetPOIAsync(cityId, poiId);
            if (poiEntity == null) return NotFound();

            this.cityInfoRepository.DeletePointOfInterestAsync(poiEntity);
            await this.cityInfoRepository.SaveChangesAsync();

            this.mailService.Send("A POI was deleted",
                $"Deleted point of interest {poiEntity.Name} in city with ID {cityId}");

            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, "An internal error occurred");
        }
        }
        }
 }
#pragma warning restore CS8602, CS0168
