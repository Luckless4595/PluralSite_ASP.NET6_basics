using CityInfo.API.Models.City;
using CityInfo.API.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AutoMapper;

namespace CityInfo.API.Controllers
{
    [ApiController]
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
        public async Task<IActionResult> GetCity(int CityId, bool includePOI = false)
        {
            var cityEntity = await this.cityInfoRepository.GetCityAsync(CityId, includePOI);
            if (cityEntity == null) return NotFound();

            if (includePOI)
                return Ok( this.mapper.Map<CityDto>(cityEntity));

            else return Ok( this.mapper.Map<CityWithoutPOIDto>(cityEntity));
        }
    }
}
