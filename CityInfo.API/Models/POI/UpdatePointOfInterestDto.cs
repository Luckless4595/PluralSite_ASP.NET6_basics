using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models.POI{

    public class UpdatePointOfInterestDto{

        [Required(ErrorMessage ="Name must be provided")]
        [MaxLength(50)]
        public string Name{ get; set; }= string.Empty;
        
        [MaxLength(200)]
        public string? Description{get;set;}
        
    }
}