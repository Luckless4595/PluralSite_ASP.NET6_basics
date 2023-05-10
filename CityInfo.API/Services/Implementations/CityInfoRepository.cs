using CityInfo.API.Services.Interfaces;
using CityInfo.API.Entities;
using CityInfo.API.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services.Implementations
{
    public class CityInfoRepository : ICityInfoRepository
    {   
        private readonly CityInfoContext context;

        public CityInfoRepository(CityInfoContext context)
        {
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await this.context.Cities.
            OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePOI)
        {
            if (includePOI){
                return await this.context.Cities
                .Include(c => c.POIs)
                .Where(c => c.Id == cityId)
                .FirstOrDefaultAsync();
            }
            else {
                return await this.context.Cities
                .Where(c => c.Id == cityId)
                .FirstOrDefaultAsync();
            } 
        }

        public async Task<IEnumerable<PointOfInterest>> GetCityPOIsAsync(int cityId)
        {
            return await this.context.POIs
                .Where(p => p.CityId == cityId)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<PointOfInterest?> GetPOIAsync(int cityId, int id)
        {
            return await this.context.POIs
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == id);
        }
    }
}
