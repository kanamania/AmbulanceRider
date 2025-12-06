using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTripRegionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromRegion",
                table: "trips",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToRegion",
                table: "trips",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromRegion",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "ToRegion",
                table: "trips");
        }
    }
}
