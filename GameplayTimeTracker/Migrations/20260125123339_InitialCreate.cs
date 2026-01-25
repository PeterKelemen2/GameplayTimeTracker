using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameplayTimeTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    ExePath = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RemoteMachines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostOs = table.Column<int>(type: "INTEGER", nullable: false),
                    HostName = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    User = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    SaveFolder = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteMachines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ThemeName = table.Column<string>(type: "TEXT", nullable: false),
                    CFooter = table.Column<string>(type: "TEXT", nullable: false),
                    CFooterFont = table.Column<string>(type: "TEXT", nullable: false),
                    CBackground = table.Column<string>(type: "TEXT", nullable: false),
                    CCard1 = table.Column<string>(type: "TEXT", nullable: false),
                    CCard2 = table.Column<string>(type: "TEXT", nullable: false),
                    CProgressBar1 = table.Column<string>(type: "TEXT", nullable: false),
                    CProgressBar2 = table.Column<string>(type: "TEXT", nullable: false),
                    ProgressBarGradOrient = table.Column<int>(type: "INTEGER", nullable: false),
                    CFont = table.Column<string>(type: "TEXT", nullable: false),
                    CRunningIndicator = table.Column<string>(type: "TEXT", nullable: false),
                    CButton = table.Column<string>(type: "TEXT", nullable: false),
                    CButtonFont = table.Column<string>(type: "TEXT", nullable: false),
                    CButtonPositive = table.Column<string>(type: "TEXT", nullable: false),
                    CButtonPositiveFont = table.Column<string>(type: "TEXT", nullable: false),
                    CButtonNegative = table.Column<string>(type: "TEXT", nullable: false),
                    CButtonNegativeFont = table.Column<string>(type: "TEXT", nullable: false),
                    CShadow = table.Column<string>(type: "TEXT", nullable: false),
                    CTransparency = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playtimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playtimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playtimes_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ThemeId = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteMachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProfileName = table.Column<string>(type: "TEXT", nullable: false),
                    StartWithSystem = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartMinimized = table.Column<bool>(type: "INTEGER", nullable: false),
                    SGDBApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    PreferSGDBImages = table.Column<bool>(type: "INTEGER", nullable: false),
                    QuickAdd = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackupOnExit = table.Column<bool>(type: "INTEGER", nullable: false),
                    DpType = table.Column<int>(type: "INTEGER", nullable: false),
                    SavingFreq = table.Column<int>(type: "INTEGER", nullable: false),
                    RemoteSavingEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_RemoteMachines_RemoteMachineId",
                        column: x => x.RemoteMachineId,
                        principalTable: "RemoteMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Settings_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Playtimes_GameId",
                table: "Playtimes",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_RemoteMachineId",
                table: "Settings",
                column: "RemoteMachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_ThemeId",
                table: "Settings",
                column: "ThemeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Playtimes");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "RemoteMachines");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
