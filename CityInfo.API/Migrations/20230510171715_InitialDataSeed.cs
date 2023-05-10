using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CityInfo.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "The largest city in the United States, famous for its iconic landmarks, world-class museums, and diverse neighborhoods.", "New York" },
                    { 2, "The second-most populous city in the United States, known for its sunny weather, entertainment industry, and cultural diversity.", "Los Angeles" },
                    { 3, "The capital of France and one of the most beautiful cities in the world, renowned for its art, architecture, and cuisine.", "Paris" }
                });

            migrationBuilder.InsertData(
                table: "POIs",
                columns: new[] { "Id", "CityId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 1, "A copper statue gifted to the US by France, representing liberty and freedom.", "Statue of Liberty" },
                    { 2, 1, "A 102-story skyscraper located in Midtown Manhattan, built in 1931.", "Empire State Building" },
                    { 3, 1, "A large urban park located in the center of Manhattan, featuring lakes, lawns, and walking paths.", "Central Park" },
                    { 4, 2, "A world-famous landmark that overlooks the city of Los Angeles, erected in 1923.", "Hollywood Sign" },
                    { 5, 2, "A public observatory and planetarium located on the south-facing slope of Mount Hollywood.", "Griffith Observatory" },
                    { 6, 2, "A large pier featuring an amusement park, an aquarium, restaurants, and shops.", "Santa Monica Pier" },
                    { 7, 3, "A wrought-iron lattice tower located on the Champ de Mars in Paris, built in 1889.", "Eiffel Tower" },
                    { 8, 3, "The world's largest art museum, featuring over 380,000 objects and 35,000 works of art.", "The Louvre" },
                    { 9, 3, "A medieval Catholic cathedral located on the Île de la Cité in the center of Paris.", "Cathédrale Notre-Dame de Paris" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "POIs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
