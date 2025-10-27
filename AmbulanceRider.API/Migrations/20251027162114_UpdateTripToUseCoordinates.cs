using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTripToUseCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_routes_RouteId",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_RouteId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "trips");

            migrationBuilder.AddColumn<double>(
                name: "FromLatitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "FromLocationId",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromLocationName",
                table: "trips",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FromLongitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToLatitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ToLocationId",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToLocationName",
                table: "trips",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ToLongitude",
                table: "trips",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FromLatitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "FromLocationName",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "FromLongitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "ToLatitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "ToLocationName",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "ToLongitude",
                table: "trips");

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "trips",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_trips_RouteId",
                table: "trips",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_routes_RouteId",
                table: "trips",
                column: "RouteId",
                principalTable: "routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
