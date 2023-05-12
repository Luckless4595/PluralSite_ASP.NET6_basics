using CityInfo.API.src.Models.City;
using CityInfo.API.src.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.src.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/cities")]
    public class CitiesController : ControllerBase 
    {
        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository citiesDataStoreIn, IMapper mapperIn)
        {
            this.cityInfoRepository = citiesDataStoreIn ?? 
                throw new ArgumentNullException(nameof(citiesDataStoreIn));

            this.mapper = mapperIn ??
                throw new ArgumentNullException(nameof(mapperIn));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPOIDto>>> GetCities(
            [FromQuery] string? cityNameFilter, // apply filter 
            [FromQuery] string? searchByCityName,
            int pageNumber = 1 , int pageSize = 10
        ){
            if (pageSize>maxCitiesPageSize)
                 pageSize = maxCitiesPageSize;
        //    note: this is not vulnerable to sql injections because EFCore sanitizes for you         
            var (cityEntities, pagingMetadata) = await this.cityInfoRepository.GetCitiesAsync(
                cityNameFilter, searchByCityName, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagingMetadata));
            var output = this.mapper.Map<IEnumerable<CityWithoutPOIDto>>(cityEntities);

            return Ok(output);
        }

        [HttpGet("{CityId}")]
        public async Task<IActionResult> GetCity(int cityId, bool includePOI = false)
        {   

            var cityEntity = await this.cityInfoRepository.GetCityAsync(cityId, includePOI);
            if (cityEntity == null) return NotFound();

            if (includePOI){
                // use the data in claims to check if the user tries to access pois outside their assigned city 
                var cityName = User.Claims.FirstOrDefault(
                    c => c.Type == "city")?.Value;

                if (!await this.cityInfoRepository.CheckCityNameMatchesCityId(cityName, cityId))
                    return Forbid();

                return Ok( this.mapper.Map<CityDto>(cityEntity));}

            else return Ok( this.mapper.Map<CityWithoutPOIDto>(cityEntity));
        }
    }
}
