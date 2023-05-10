using CityInfo.API.Entities;
using CityInfo.API.DbContexts;
using CityInfo.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services.Implementations
{
    public class CityInfoRepository : ICityInfoRepository
    {   
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET
        public async Task<bool> SaveChangesAsync(){
            return (await _context.SaveChangesAsync() >=0);
        }

        public async Task<bool> CheckCityExistsAsync(int cityId){
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOI)
        {
            if (includePOI)
                return await _context.Cities
                        .Include(c => c.POIs)
                        .Where(c => c.Id == cityId)
                        .FirstOrDefaultAsync();
            

            else    
                return await _context.Cities
                  .Where(c => c.Id == cityId)
                  .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointOfInterest>> GetCityPOIsAsync(int cityId)
        {
            return await _context.POIs
                .Where(p => p.CityId == cityId)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<PointOfInterest?> GetPOIAsync(int cityId, int id)
        {
            return await _context.POIs
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == id);
        }

        //Manipulations
        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest newPOI){

            var city = await GetCityAsync(cityId, false);
            if (city != null) city.POIs.Add(newPOI);
        }

    }
}
