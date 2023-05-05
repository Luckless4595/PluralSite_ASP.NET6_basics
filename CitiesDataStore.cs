namespace CityInfo.API{
    public class CitiesDataStore{
        public List<CityDto> Cities {get; set;}

        public CitiesDataStore() {
            this.Cities = new List<CityDto>(){
                new CityDto(){
                    Id = 0,
                    Name = "NY",
                    Description = "The one that's hella expensive"
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
