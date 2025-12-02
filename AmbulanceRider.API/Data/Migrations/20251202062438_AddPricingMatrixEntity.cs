using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AmbulanceRider.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingMatrixEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PricingMatrixId",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "trips",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "pricing_matrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MinWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxWeight = table.Column<decimal>(type: "numeric", nullable: false),
                    MinHeight = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxHeight = table.Column<decimal>(type: "numeric", nullable: false),
                    MinLength = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxLength = table.Column<decimal>(type: "numeric", nullable: false),
                    MinWidth = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxWidth = table.Column<decimal>(type: "numeric", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CompanyId = table.Column<int>(type: "integer", nullable: true),
                    VehicleTypeId = table.Column<int>(type: "integer", nullable: true),
                    TripTypeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pricing_matrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pricing_matrices_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_pricing_matrices_trip_types_TripTypeId",
                        column: x => x.TripTypeId,
                        principalTable: "trip_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_pricing_matrices_vehicle_types_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "vehicle_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trips_PricingMatrixId",
                table: "trips",
                column: "PricingMatrixId");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_matrices_CompanyId",
                table: "pricing_matrices",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_matrices_TripTypeId",
                table: "pricing_matrices",
                column: "TripTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_pricing_matrices_VehicleTypeId",
                table: "pricing_matrices",
                column: "VehicleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_pricing_matrices_PricingMatrixId",
                table: "trips",
                column: "PricingMatrixId",
                principalTable: "pricing_matrices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_pricing_matrices_PricingMatrixId",
                table: "trips");

            migrationBuilder.DropTable(
                name: "pricing_matrices");

            migrationBuilder.DropIndex(
                name: "IX_trips_PricingMatrixId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "PricingMatrixId",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "trips");
        }
    }
}
