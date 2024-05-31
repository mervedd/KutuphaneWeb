using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplicationKendim.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sayi",
                table: "KitapBilgileriTablosu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Sayi",
                table: "KitapBilgileriTablosu",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
