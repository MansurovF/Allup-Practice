using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pustok_BackProject.Migrations
{
    public partial class AddedProductssTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MostViewed",
                table: "Products",
                newName: "IsMostviewProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsMostviewProducts",
                table: "Products",
                newName: "MostViewed");
        }
    }
}
