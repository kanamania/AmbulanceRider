using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTripManagementEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_types_vehicles_VehicleId",
                table: "vehicle_types");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_types_VehicleId",
                table: "vehicle_types");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "vehicle_types");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "vehicles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "vehicles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AutoApproved",
                table: "trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoStarted",
                table: "trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "CompletionAccuracy",
                table: "trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CompletionLatitude",
                table: "trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CompletionLongitude",
                table: "trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EstimatedDistance",
                table: "trips",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDuration",
                table: "trips",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptimizedRoute",
                table: "trips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "trips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresGpsVerification",
                table: "trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RoutePolyline",
                table: "trips",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureUrl",
                table: "trips",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldValues = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    NewValues = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    AffectedProperties = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserRole = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "performance_logs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Endpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    ResponseTimeMs = table.Column<double>(type: "double precision", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequestSize = table.Column<long>(type: "bigint", nullable: true),
                    ResponseSize = table.Column<long>(type: "bigint", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_performance_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Action",
                table: "audit_logs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EntityId",
                table: "audit_logs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EntityType",
                table: "audit_logs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Timestamp",
                table: "audit_logs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_UserId",
                table: "audit_logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_performance_logs_Endpoint",
                table: "performance_logs",
                column: "Endpoint");

            migrationBuilder.CreateIndex(
                name: "IX_performance_logs_StatusCode",
                table: "performance_logs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_performance_logs_Timestamp",
                table: "performance_logs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "performance_logs");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "AutoApproved",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "AutoStarted",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "CompletionAccuracy",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "CompletionLatitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "CompletionLongitude",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "EstimatedDistance",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "EstimatedDuration",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "OptimizedRoute",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "RequiresGpsVerification",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "RoutePolyline",
                table: "trips");

            migrationBuilder.DropColumn(
                name: "SignatureUrl",
                table: "trips");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "vehicles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "vehicle_types",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_types_VehicleId",
                table: "vehicle_types",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_types_vehicles_VehicleId",
                table: "vehicle_types",
                column: "VehicleId",
                principalTable: "vehicles",
                principalColumn: "Id");
        }
    }
}
