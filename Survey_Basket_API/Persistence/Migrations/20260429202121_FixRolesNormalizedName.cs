using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey_Basket_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixRolesNormalizedName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4f570688-9ed8-474f-9975-1317c87f1f7f",
                column: "NormalizedName",
                value: "ADMIN");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce197ed8-850f-4272-880f-5b929837ff9e",
                column: "NormalizedName",
                value: "MEMBER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4f570688-9ed8-474f-9975-1317c87f1f7f",
                column: "NormalizedName",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce197ed8-850f-4272-880f-5b929837ff9e",
                column: "NormalizedName",
                value: null);
        }
    }
}
