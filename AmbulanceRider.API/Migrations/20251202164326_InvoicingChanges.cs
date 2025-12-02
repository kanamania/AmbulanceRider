using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    /// <inheritdoc />
    public partial class InvoicingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_trips_CreatedBy",
                table: "trips",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_trips_users_CreatedBy",
                table: "trips",
                column: "CreatedBy",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trips_users_CreatedBy",
                table: "trips");

            migrationBuilder.DropIndex(
                name: "IX_trips_CreatedBy",
                table: "trips");
        }
    }
}
