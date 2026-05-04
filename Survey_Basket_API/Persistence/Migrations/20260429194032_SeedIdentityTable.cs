using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Survey_Basket_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4f570688-9ed8-474f-9975-1317c87f1f7f", "e566a007-a817-454d-bb8f-10d1f09e5715", false, false, "Admin", null },
                    { "ce197ed8-850f-4272-880f-5b929837ff9e", "e108b4c9-9c16-4396-a91a-39a3b9e9c11d", true, false, "Member", null }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e232de14-def2-4d76-9dcb-05f0eb7619a1", 0, "5abffc5a-7e3a-4963-a4e3-755bdda4e3d5", "admin@survey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAEBYocXB8KArWfHdt/nkNVt4ZJl+6o09BRIdEnR2WXpJDyiTwpguILscEOmiMTJB96g==", null, false, "AC6C88E23E674BC0AA53B4B195D91D56", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 2, "permissions", "polls:add", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 3, "permissions", "polls:update", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 4, "permissions", "polls:delete", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 5, "permissions", "questions:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 6, "permissions", "questions:add", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 7, "permissions", "questions:update", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 8, "permissions", "users:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 9, "permissions", "users:add", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 10, "permissions", "users:update", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 11, "permissions", "roles:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 12, "permissions", "roles:add", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 13, "permissions", "roles:update", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 14, "permissions", "results:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "4f570688-9ed8-474f-9975-1317c87f1f7f", "e232de14-def2-4d76-9dcb-05f0eb7619a1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce197ed8-850f-4272-880f-5b929837ff9e");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4f570688-9ed8-474f-9975-1317c87f1f7f", "e232de14-def2-4d76-9dcb-05f0eb7619a1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4f570688-9ed8-474f-9975-1317c87f1f7f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e232de14-def2-4d76-9dcb-05f0eb7619a1");
        }
    }
}
