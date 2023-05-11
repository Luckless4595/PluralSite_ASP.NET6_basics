using CityInfo.API.Entities;

using CityInfo.API.Services.Implementations;
namespace CityInfo.API.Services.Interfaces{
    public interface ICityInfoRepository{

        //GET
        Task<bool> SaveChangesAsync();
        Task<bool> CheckCityExistsAsync(int cityId);
        Task<(IEnumerable<City>,PagingMetadata)> GetCitiesAsync(
            string? cityNameFilter, string? searchByCityName, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int CityId, bool includePOI);
        Task<(IEnumerable<PointOfInterest>, PagingMetadata)> GetCityPOIsAsync(
            int cityId, int pageNumber, int pageSize);
        Task<PointOfInterest?> GetPOIAsync(int CityId, int Id);

        //Manipulations
        // Note actually only i/o operations (get) need to be async, delete and add are in memory
        // so not async, we only made add async here because we wrote it to get POI in the method
        // if we wrote get outside then passed it to add, we would not need add to be async
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest newPOI);
        void DeletePointOfInterestAsync(PointOfInterest poi);
    }
}