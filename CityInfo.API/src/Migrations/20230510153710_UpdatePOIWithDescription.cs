using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityInfo.API.src.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePOIWithDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "POIs",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "POIs");
        }
    }
}
