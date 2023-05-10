using Microsoft.EntityFrameworkCore;
using CityInfo.API.Entities;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<PointOfInterest> POIs { get; set; } = null!;

        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
        }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("New York")
            {
                Id = 1,
                Description = "The largest city in the United States, famous for its iconic landmarks, world-class museums, and diverse neighborhoods."
            },
            new City("Los Angeles")
            {
                Id = 2,
                Description = "The second-most populous city in the United States, known for its sunny weather, entertainment industry, and cultural diversity."
            },
            new City("Paris")
            {
                Id = 3,
                Description = "The capital of France and one of the most beautiful cities in the world, renowned for its art, architecture, and cuisine."
            }
        );

        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Statue of Liberty")
            {
                Id = 1,
                Description = "A copper statue gifted to the US by France, representing liberty and freedom.",
                CityId = 1
            },
            new PointOfInterest("Empire State Building")
            {
                Id = 2,
                Description = "A 102-story skyscraper located in Midtown Manhattan, built in 1931.",
                CityId = 1
            },
            new PointOfInterest("Central Park")
            {
                Id = 3,
                Description = "A large urban park located in the center of Manhattan, featuring lakes, lawns, and walking paths.",
                CityId = 1
            },
            new PointOfInterest("Hollywood Sign")
            {
                Id = 4,
                Description = "A world-famous landmark that overlooks the city of Los Angeles, erected in 1923.",
                CityId = 2
            },
            new PointOfInterest("Griffith Observatory")
            {
                Id = 5,
                Description = "A public observatory and planetarium located on the south-facing slope of Mount Hollywood.",
                CityId = 2
            },
            new PointOfInterest("Santa Monica Pier")
            {
                Id = 6,
                Description = "A large pier featuring an amusement park, an aquarium, restaurants, and shops.",
                CityId = 2
            },
            new PointOfInterest("Eiffel Tower")
            {
                Id = 7,
                Description = "A wrought-iron lattice tower located on the Champ de Mars in Paris, built in 1889.",
                CityId = 3
            },
            new PointOfInterest("The Louvre")
            {
                Id = 8,
                Description = "The world's largest art museum, featuring over 380,000 objects and 35,000 works of art.",
                CityId = 3
            },
            new PointOfInterest("Cathédrale Notre-Dame de Paris")
            {
                Id = 9,
                Description = "A medieval Catholic cathedral located on the Île de la Cité in the center of Paris.",
                CityId = 3
            }
        );

        base.OnModelCreating(modelBuilder);
    }

    }
}
