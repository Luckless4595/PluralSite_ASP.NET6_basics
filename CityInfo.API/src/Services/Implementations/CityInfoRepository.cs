using CityInfo.API.src.Entities;
using CityInfo.API.src.DbContexts;
using CityInfo.API.src.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.src.Services.Implementations
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
        //overload
        #pragma warning disable CS8602
        public async Task<(IEnumerable<City>, PagingMetadata)> GetCitiesAsync(
            string? cityNameFilter, string? searchByCityName, int pageNumber, int pageSize)
        {
            bool noFilter = string.IsNullOrWhiteSpace(cityNameFilter);
            bool noQuery = string.IsNullOrWhiteSpace (searchByCityName);

            var citiesCollection = _context.Cities as IQueryable<City>;    

            if (!noFilter){
                cityNameFilter = cityNameFilter.Replace(" ","").ToLower();
                citiesCollection = citiesCollection.Where
                (c => c.Name.ToLower().Replace(" ","") == cityNameFilter);
                    }

            if (! noQuery ){
                searchByCityName = searchByCityName.Replace(" ","").ToLower();
                citiesCollection = citiesCollection.Where
                (c => c.Name.Replace(" ","").ToLower().Contains(searchByCityName));
            }

            // note: if both params are entered, you may think that returning the filtered
            // collection first then searching that collection would be a good idea
            // it is not. becuase that search means putting stuff in memory that you might
            // not be able to handle. Just keep it like this so the commands apply in DB
            // the Iqueryable contains all commads we passed and executes only when 
            // we iterate (in this case at Tolistasync()). So performance = GOOD
            
            var totalItemCount = await citiesCollection.CountAsync();
            var metadata = new PagingMetadata(totalItemCount, pageSize, pageNumber);
            
            var output = await citiesCollection
                        .OrderBy(c => c.Id)
                        .Skip(pageSize*(pageNumber - 1))
                        .Take(pageSize)
                        .ToListAsync();
            
            return (output, metadata);
        }
        #pragma warning restore CS8602

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

        public async Task<(IEnumerable<PointOfInterest>, PagingMetadata)> GetCityPOIsAsync(int cityId, int pageNumber, int pageSize)
        {
            var poisCollection = _context.POIs.Where(p => p.CityId == cityId);
            var totalItemCount = await poisCollection.CountAsync();
            var metadata = new PagingMetadata(totalItemCount, pageSize, pageNumber);

            var output = await poisCollection
                .OrderBy(p => p.Id)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (output, metadata);
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

        public void DeletePointOfInterestAsync(PointOfInterest poi){
            _context.POIs.Remove(poi);
        }

    }
}
