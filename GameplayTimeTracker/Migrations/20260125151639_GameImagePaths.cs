using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameplayTimeTracker.Migrations
{
    /// <inheritdoc />
    public partial class GameImagePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeroPath",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconPath",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeroPath",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IconPath",
                table: "Games");
        }
    }
}
