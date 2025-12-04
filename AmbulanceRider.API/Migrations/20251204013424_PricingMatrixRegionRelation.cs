using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    /// <inheritdoc />
    public partial class PricingMatrixRegionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pricing_matrices_Regions_RegionId",
                table: "pricing_matrices");

            migrationBuilder.AddForeignKey(
                name: "FK_pricing_matrices_Regions_RegionId",
                table: "pricing_matrices",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_pricing_matrices_Regions_RegionId",
                table: "pricing_matrices");

            migrationBuilder.AddForeignKey(
                name: "FK_pricing_matrices_Regions_RegionId",
                table: "pricing_matrices",
                column: "RegionId",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
