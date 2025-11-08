using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_Roles_roleID",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Person_ClientID",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Sessions_SessionID",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Person_roleID",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "roleID",
                table: "Person");

            migrationBuilder.RenameColumn(
                name: "ClientID",
                table: "Tickets",
                newName: "PersonID");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ClientID",
                table: "Tickets",
                newName: "IX_Tickets_PersonID");

            migrationBuilder.AlterColumn<int>(
                name: "SessionID",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsStudent",
                table: "Person",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActor",
                table: "Person",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsClient",
                table: "Person",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirector",
                table: "Person",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Person",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "Person",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Person_PersonID",
                table: "Tickets",
                column: "PersonID",
                principalTable: "Person",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Sessions_SessionID",
                table: "Tickets",
                column: "SessionID",
                principalTable: "Sessions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Person_PersonID",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Sessions_SessionID",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsActor",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "IsClient",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "IsDirector",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "Person");

            migrationBuilder.RenameColumn(
                name: "PersonID",
                table: "Tickets",
                newName: "ClientID");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_PersonID",
                table: "Tickets",
                newName: "IX_Tickets_ClientID");

            migrationBuilder.AlterColumn<int>(
                name: "SessionID",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsStudent",
                table: "Person",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Person",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "roleID",
                table: "Person",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Person_roleID",
                table: "Person",
                column: "roleID");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Roles_roleID",
                table: "Person",
                column: "roleID",
                principalTable: "Roles",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Person_ClientID",
                table: "Tickets",
                column: "ClientID",
                principalTable: "Person",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Sessions_SessionID",
                table: "Tickets",
                column: "SessionID",
                principalTable: "Sessions",
                principalColumn: "ID");
        }
    }
}
