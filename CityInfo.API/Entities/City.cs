using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities{
    public class City{

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        
        [Required]
        [MaxLength(50)]
        public string Name{ get; set; }
        
        [MaxLength(200)]
        public string? Description{get;set;}
        
        public City(string name){
            this.Name = name;
        }
        
        //Points of Interest
        public ICollection<PointOfInterest> POIs {get; set;}
        = new List<PointOfInterest>();
    }
}