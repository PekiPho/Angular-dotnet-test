using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class CommentUpdate20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentVote_Comments_CommentId",
                table: "CommentVote");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentVote_Users_UserId",
                table: "CommentVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentVote",
                table: "CommentVote");

            migrationBuilder.RenameTable(
                name: "CommentVote",
                newName: "CommentVotes");

            migrationBuilder.RenameIndex(
                name: "IX_CommentVote_UserId",
                table: "CommentVotes",
                newName: "IX_CommentVotes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentVote_CommentId",
                table: "CommentVotes",
                newName: "IX_CommentVotes_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentVotes",
                table: "CommentVotes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVotes_Comments_CommentId",
                table: "CommentVotes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVotes_Users_UserId",
                table: "CommentVotes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentVotes_Comments_CommentId",
                table: "CommentVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentVotes_Users_UserId",
                table: "CommentVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentVotes",
                table: "CommentVotes");

            migrationBuilder.RenameTable(
                name: "CommentVotes",
                newName: "CommentVote");

            migrationBuilder.RenameIndex(
                name: "IX_CommentVotes_UserId",
                table: "CommentVote",
                newName: "IX_CommentVote_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentVotes_CommentId",
                table: "CommentVote",
                newName: "IX_CommentVote_CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentVote",
                table: "CommentVote",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVote_Comments_CommentId",
                table: "CommentVote",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVote_Users_UserId",
                table: "CommentVote",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
