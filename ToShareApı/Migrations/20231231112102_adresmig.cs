using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToShareApı.Migrations
{
    /// <inheritdoc />
    public partial class adresmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adres",
                table: "Posts");
        }
    }
}
