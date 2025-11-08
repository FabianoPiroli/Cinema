using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Migrations
{
    /// <inheritdoc />
    public partial class RolesManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Person_PersonID",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_PersonID",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "PersonID",
                table: "Roles");

            migrationBuilder.CreateTable(
                name: "PersonRole",
                columns: table => new
                {
                    PersonsID = table.Column<int>(type: "int", nullable: false),
                    RolesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonRole", x => new { x.PersonsID, x.RolesID });
                    table.ForeignKey(
                        name: "FK_PersonRole_Person_PersonsID",
                        column: x => x.PersonsID,
                        principalTable: "Person",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonRole_Roles_RolesID",
                        column: x => x.RolesID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonRole_RolesID",
                table: "PersonRole",
                column: "RolesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonRole");

            migrationBuilder.AddColumn<int>(
                name: "PersonID",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_PersonID",
                table: "Roles",
                column: "PersonID");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Person_PersonID",
                table: "Roles",
                column: "PersonID",
                principalTable: "Person",
                principalColumn: "ID");
        }
    }
}
