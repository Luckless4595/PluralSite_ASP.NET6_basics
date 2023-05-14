using CityInfo.API.src.Models.City;
using CityInfo.API.src.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.src.Controllers
{
    /// <summary>
    /// Controller for managing cities.
    /// </summary>
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cities")]
    public class CitiesController : ControllerBase 
    {
            private readonly ICityInfoRepository cityInfoRepository;
            private readonly IMapper mapper;
            const int maxCitiesPageSize = 20;

            /// <summary>
            /// Initializes a new instance of the <see cref="CitiesController"/> class.
            /// </summary>
            /// <param name="citiesDataStoreIn">The city information repository.</param>
            /// <param name="mapperIn">The mapper for mapping city entities to DTOs.</param>
            public CitiesController(ICityInfoRepository citiesDataStoreIn, IMapper mapperIn)
            {
                this.cityInfoRepository = citiesDataStoreIn ?? 
                    throw new ArgumentNullException(nameof(citiesDataStoreIn));

                this.mapper = mapperIn ??
                    throw new ArgumentNullException(nameof(mapperIn));
            }

            /// <summary>
            /// Retrieves a list of cities.
            /// </summary>
            /// <param name="cityNameFilter">Optional. Filters cities by name.</param>
            /// <param name="searchByCityName">Optional. Searches cities by name.</param>
            /// <param name="pageNumber">Optional. The page number to retrieve (default: 1).</param>
            /// <param name="pageSize">Optional. The number of cities per page (default: 10).</param>
            /// <returns>A list of cities without points of interest.</returns>
            [HttpGet]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<IEnumerable<CityWithoutPOIDto>>> GetCities(
                [FromQuery] string? cityNameFilter, // apply filter 
                [FromQuery] string? searchByCityName,
                int pageNumber = 1 , int pageSize = 10
            ){
                if (pageSize > maxCitiesPageSize)
                    pageSize = maxCitiesPageSize;
                // note: this is not vulnerable to sql injections because EFCore sanitizes for you         
                var (cityEntities, pagingMetadata) = await this.cityInfoRepository.GetCitiesAsync(
                    cityNameFilter, searchByCityName, pageNumber, pageSize);

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagingMetadata));
                var output = this.mapper.Map<IEnumerable<CityWithoutPOIDto>>(cityEntities);

                return Ok(output);
            }

            /// <summary>
            /// Retrieves a city by its ID.
            /// </summary>
            /// <param name="cityId">The ID of the city to retrieve.</param>
            /// <param name="includePOI">Optional. Determines whether to include points of interest (default: false).</param>
            /// <returns>The city information.</returns>
            [HttpGet("{CityId}")]
            public async Task<IActionResult> GetCity(int cityId, bool includePOI = false)
            {   
                var cityEntity = await this.cityInfoRepository.GetCityAsync(cityId, includePOI);
                if (cityEntity == null) return NotFound();

                if (includePOI)
                {
                    // use the data in claims to check if the user tries to access pois outside their assigned city 
                    var cityName = User.Claims.FirstOrDefault(
                        c => c.Type == "city")?.Value;

                    // throw new Exception(cityName);
                    if (!await this.cityInfoRepository.CheckCityNameMatchesCityId(cityName, cityId))
                    {
                        return Forbid();
                    }

                    return Ok(this.mapper.Map<CityDto>(cityEntity));
                }
                else return Ok(this.mapper.Map<CityWithoutPOIDto>(cityEntity));
        }
    }

}
