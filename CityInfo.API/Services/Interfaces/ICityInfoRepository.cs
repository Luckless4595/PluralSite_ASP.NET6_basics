using CityInfo.API.Entities;

namespace CityInfo.API.Services.Interfaces{
    public interface ICityInfoRepository{

        //GET
        Task<bool> SaveChangesAsync();
        Task<bool> CheckCityExistsAsync(int cityId);
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int CityId, bool includePOI);
        Task<IEnumerable<PointOfInterest>> GetCityPOIsAsync(int CityId);
        Task<PointOfInterest?> GetPOIAsync(int CityId, int Id);

        //Manipulations
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest newPOI);
    }
}