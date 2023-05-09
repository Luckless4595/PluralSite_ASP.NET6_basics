using Microsoft.EntityFrameworkCore;
using CityInfo.API.Entities;

namespace CityInfo.API.DbContexts{
    public class CityInfoContext : DbContext{

        // DbSet ensures its never null, the = null! disables that warning because it's ok
        public DbSet<City> Cities {get; set;} = null!;
        public DbSet <PointOfInterest> POIs {get; set;} = null!;

        public CityInfoContext(DbContextOptions<CityInfoContext> options)
        :base(options){} //basically called super(options)

    }
}