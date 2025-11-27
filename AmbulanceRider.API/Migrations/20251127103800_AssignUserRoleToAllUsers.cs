using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    public partial class AssignUserRoleToAllUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Ensure User role exists
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT '22222222-2222-2222-2222-222222222222', 'User', 'USER', NEWID()
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""NormalizedName"" = 'USER');

                -- Assign User role to all users without roles
                INSERT INTO ""AspNetUserRoles"" (""UserId"", ""RoleId"", ""Discriminator"")
                SELECT u.""Id"", r.""Id"", 'UserRole'
                FROM ""AspNetUsers"" u
                CROSS JOIN ""AspNetRoles"" r
                WHERE r.""NormalizedName"" = 'USER'
                  AND NOT EXISTS (
                    SELECT 1 FROM ""AspNetUserRoles"" ur 
                    WHERE ur.""UserId"" = u.""Id""
                  );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Remove User role from all users
                DELETE FROM ""AspNetUserRoles""
                WHERE ""RoleId"" IN (
                    SELECT ""Id"" FROM ""AspNetRoles"" WHERE ""NormalizedName"" = 'USER'
                );
            ");
        }
    }
}
