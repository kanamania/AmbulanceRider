using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationReferencesFromTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_locations_FromLocationId",
                table: "trips");

            migrationBuilder.DropForeignKey(
                name: "FK_trips_locations_ToLocationId",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_FromLocationId",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_ToLocationId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "trips");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromLocationId",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToLocationId",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_trips_FromLocationId",
                table: "trips",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_trips_ToLocationId",
                table: "trips",
                column: "ToLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_locations_FromLocationId",
                table: "trips",
                column: "FromLocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_trips_locations_ToLocationId",
                table: "trips",
                column: "ToLocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
