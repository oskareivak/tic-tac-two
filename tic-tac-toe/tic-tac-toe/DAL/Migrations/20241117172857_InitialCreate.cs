using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    BoardSize = table.Column<int>(type: "INTEGER", nullable: false),
                    GridSize = table.Column<int>(type: "INTEGER", nullable: false),
                    WinCondition = table.Column<int>(type: "INTEGER", nullable: false),
                    WhoStarts = table.Column<int>(type: "INTEGER", nullable: false),
                    MovePieceAfterNMoves = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfPiecesPerPlayer = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAtDateTime = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 10240, nullable: false),
                    ConfigurationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedGames_Configurations_ConfigurationId",
                        column: x => x.ConfigurationId,
                        principalTable: "Configurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedGames_ConfigurationId",
                table: "SavedGames",
                column: "ConfigurationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedGames");

            migrationBuilder.DropTable(
                name: "Configurations");
        }
    }
}
