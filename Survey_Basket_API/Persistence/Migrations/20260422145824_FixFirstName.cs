using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey_Basket_API.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixFirstName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirsName",
                table: "AspNetUsers",
                newName: "FirstName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "FirsName");
        }
    }
}
