using CityInfo.API.src.Models.POI;

namespace CityInfo.API.src.Models.City
{
    /// <summary>
    /// Data transfer object for representing a city without points of interest.
    /// </summary>
    public class CityWithoutPOIDto
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
    }
}
