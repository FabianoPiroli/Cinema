using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Movies_MovieID",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Movies_AgeRating_AgeRatingID",
                table: "Movies");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Movies_MovieID",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Movies_MovieID1",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_MovieID",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_MovieID1",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Genres_MovieID",
                table: "Genres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgeRating",
                table: "AgeRating");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "MovieID1",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "Genres");

            migrationBuilder.RenameTable(
                name: "AgeRating",
                newName: "AgeRatings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgeRatings",
                table: "AgeRatings",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "MovieActors",
                columns: table => new
                {
                    ActedMoviesID = table.Column<int>(type: "int", nullable: false),
                    ActorsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieActors", x => new { x.ActedMoviesID, x.ActorsID });
                    table.ForeignKey(
                        name: "FK_MovieActors_Movies_ActedMoviesID",
                        column: x => x.ActedMoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieActors_Person_ActorsID",
                        column: x => x.ActorsID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieDirectors",
                columns: table => new
                {
                    DirectedMoviesID = table.Column<int>(type: "int", nullable: false),
                    DirectorsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieDirectors", x => new { x.DirectedMoviesID, x.DirectorsID });
                    table.ForeignKey(
                        name: "FK_MovieDirectors_Movies_DirectedMoviesID",
                        column: x => x.DirectedMoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieDirectors_Person_DirectorsID",
                        column: x => x.DirectorsID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    GenresID = table.Column<int>(type: "int", nullable: false),
                    MoviesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.GenresID, x.MoviesID });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenresID",
                        column: x => x.GenresID,
                        principalTable: "Genres",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MoviesID",
                        column: x => x.MoviesID,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_ActorsID",
                table: "MovieActors",
                column: "ActorsID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieDirectors_DirectorsID",
                table: "MovieDirectors",
                column: "DirectorsID");

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_MoviesID",
                table: "MovieGenres",
                column: "MoviesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_AgeRatings_AgeRatingID",
                table: "Movies",
                column: "AgeRatingID",
                principalTable: "AgeRatings",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_AgeRatings_AgeRatingID",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "MovieActors");

            migrationBuilder.DropTable(
                name: "MovieDirectors");

            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AgeRatings",
                table: "AgeRatings");

            migrationBuilder.RenameTable(
                name: "AgeRatings",
                newName: "AgeRating");

            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "Person",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovieID1",
                table: "Person",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "Genres",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AgeRating",
                table: "AgeRating",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_MovieID",
                table: "Person",
                column: "MovieID");

            migrationBuilder.CreateIndex(
                name: "IX_Person_MovieID1",
                table: "Person",
                column: "MovieID1");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_MovieID",
                table: "Genres",
                column: "MovieID");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Movies_MovieID",
                table: "Genres",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_AgeRating_AgeRatingID",
                table: "Movies",
                column: "AgeRatingID",
                principalTable: "AgeRating",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Movies_MovieID",
                table: "Person",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Movies_MovieID1",
                table: "Person",
                column: "MovieID1",
                principalTable: "Movies",
                principalColumn: "ID");
        }
    }
}
