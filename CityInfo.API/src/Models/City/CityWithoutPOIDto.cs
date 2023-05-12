using CityInfo.API.src.Models.POI;

namespace CityInfo.API.src.Models.City{

    public class CityWithoutPOIDto{

        public int Id {get; set;}
        public string Name{ get; set; }= string.Empty;
        public string? Description{get;set;}
    
    }
}