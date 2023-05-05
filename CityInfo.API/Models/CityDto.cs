namespace CityInfo.API.Models{

    public class CityDto{

        public int Id {get; set;}
        public string Name{ get; set; }= string.Empty;
        public string? Description{get;set;}
        //Points of Interest
        public int NumberOfPOIs {
            get{
                return this.POIs.Count;
            }}
        public ICollection<PointOfInterestDto> POIs {get; set;}
        = new List<PointOfInterestDto>();
    }
}