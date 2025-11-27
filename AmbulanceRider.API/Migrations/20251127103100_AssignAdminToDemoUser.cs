using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmbulanceRider.API.Migrations
{
    public partial class AssignAdminToDemoUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Ensure Admin role exists
                INSERT INTO ""AspNetRoles"" (""Id"", ""Name"", ""NormalizedName"", ""ConcurrencyStamp"")
                SELECT '11111111-1111-1111-1111-111111111111', 'Admin', 'ADMIN', NEWID()
                WHERE NOT EXISTS (SELECT 1 FROM ""AspNetRoles"" WHERE ""NormalizedName"" = 'ADMIN');

                -- Assign Admin role to demo@gmail.com
                INSERT INTO ""AspNetUserRoles"" (""UserId"", ""RoleId"", ""Discriminator"")
                SELECT u.""Id"", r.""Id"", 'UserRole'
                FROM ""AspNetUsers"" u
                CROSS JOIN ""AspNetRoles"" r
                WHERE u.""Email"" = 'demo@gmail.com' 
                  AND r.""NormalizedName"" = 'ADMIN'
                  AND NOT EXISTS (
                    SELECT 1 FROM ""AspNetUserRoles"" ur 
                    WHERE ur.""UserId"" = u.""Id"" AND ur.""RoleId"" = r.""Id""
                  );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                -- Remove Admin role from demo@gmail.com
                DELETE FROM ""AspNetUserRoles""
                WHERE ""UserId"" IN (
                    SELECT ""Id"" FROM ""AspNetUsers"" WHERE ""Email"" = 'demo@gmail.com'
                ) AND ""RoleId"" IN (
                    SELECT ""Id"" FROM ""AspNetRoles"" WHERE ""NormalizedName"" = 'ADMIN'
                );
            ");
        }
    }
}
