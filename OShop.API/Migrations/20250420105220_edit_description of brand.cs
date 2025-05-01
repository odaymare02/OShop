using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OShop.API.Migrations
{
    /// <inheritdoc />
    public partial class edit_descriptionofbrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descrition",
                table: "brands",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "brands",
                newName: "Descrition");
        }
    }
}
