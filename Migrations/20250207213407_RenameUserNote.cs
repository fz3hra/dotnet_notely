using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_notely.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotes",
                table: "UserNotes");

            migrationBuilder.RenameTable(
                name: "UserNotes",
                newName: "NoteShares");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotes_UserId_NoteId",
                table: "NoteShares",
                newName: "IX_NoteShares_UserId_NoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NoteShares",
                table: "NoteShares",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NoteShares",
                table: "NoteShares");

            migrationBuilder.RenameTable(
                name: "NoteShares",
                newName: "UserNotes");

            migrationBuilder.RenameIndex(
                name: "IX_NoteShares_UserId_NoteId",
                table: "UserNotes",
                newName: "IX_UserNotes_UserId_NoteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotes",
                table: "UserNotes",
                column: "Id");
        }
    }
}
