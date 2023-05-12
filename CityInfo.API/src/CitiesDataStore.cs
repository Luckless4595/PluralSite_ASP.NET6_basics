using CityInfo.API.src.Models.City;
using CityInfo.API.src.Models.POI;

namespace CityInfo.API{
    public class CitiesDataStore{
        public List<CityDto> Cities {get; set;}
        // public static CitiesDataStore Current {get;} = new CitiesDataStore();
        public CitiesDataStore() {
            this.Cities = new List<CityDto>(){
                new CityDto(){
                    Id = 0,
                    Name = "NY",
                    Description = "The one that's hella expensive",
                    POIs = new List<PointOfInterestDto>(){
                        new PointOfInterestDto(){
                            Id = 0,
                            Name = "Central Park",
                            Description = "An average park that is somehow hyped"
                        },
                        new PointOfInterestDto(){
                            Id = 1,
                            Name = "Statue of liberty",
                            Description = "Ok theres this whole legal debate on whether it belongs to NY or not but hey"
                        }
                    }
                },
                new CityDto(){
                    Id = 1,
                    Name = "LA",
                    Description = "The one that's even worse"
                }
            };
        }
    }
}
