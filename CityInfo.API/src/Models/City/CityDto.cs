using CityInfo.API.src.Models.POI;

namespace CityInfo.API.src.Models.City
{
    /// <summary>
    /// Data transfer object for representing a city.
    /// </summary>
    public class CityDto
    {
        /// <summary>
        /// Gets or sets the ID of the city.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the city.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the number of points of interest in the city.
        /// </summary>
        public int NumberOfPOIs
        {
            get
            {
                return this.POIs.Count;
            }
        }

        /// <summary>
        /// Gets or sets the collection of points of interest in the city.
        /// </summary>
        public ICollection<PointOfInterestDto> POIs { get; set; }
            = new List<PointOfInterestDto>();
    }
}
