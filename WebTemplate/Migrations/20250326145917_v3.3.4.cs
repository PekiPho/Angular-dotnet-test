using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class v334 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "Communities",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //         Description = table.Column<string>(type: "nvarchar(max)", maxLength: 20000, nullable: true),
            //         CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Communities", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Users",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //         Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         DateOfAccountCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Users", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Posts",
            //     columns: table => new
            //     {
            //         Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //         CommunityId = table.Column<int>(type: "int", nullable: true),
            //         Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
            //         Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
            //         UserId = table.Column<int>(type: "int", nullable: true),
            //         Vote = table.Column<int>(type: "int", nullable: false),
            //         DateOfPost = table.Column<DateTime>(type: "datetime2", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Posts", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_Posts_Communities_CommunityId",
            //             column: x => x.CommunityId,
            //             principalTable: "Communities",
            //             principalColumn: "Id");
            //         table.ForeignKey(
            //             name: "FK_Posts_Users_UserId",
            //             column: x => x.UserId,
            //             principalTable: "Users",
            //             principalColumn: "Id");
            //     });

            // migrationBuilder.CreateTable(
            //     name: "UserModerating",
            //     columns: table => new
            //     {
            //         CommunityID = table.Column<int>(type: "int", nullable: false),
            //         UserID = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_UserModerating", x => new { x.CommunityID, x.UserID });
            //         table.ForeignKey(
            //             name: "FK_UserModerating_Communities_CommunityID",
            //             column: x => x.CommunityID,
            //             principalTable: "Communities",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_UserModerating_Users_UserID",
            //             column: x => x.UserID,
            //             principalTable: "Users",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "UserSubscribed",
            //     columns: table => new
            //     {
            //         CommunityID = table.Column<int>(type: "int", nullable: false),
            //         UserID = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_UserSubscribed", x => new { x.CommunityID, x.UserID });
            //         table.ForeignKey(
            //             name: "FK_UserSubscribed_Communities_CommunityID",
            //             column: x => x.CommunityID,
            //             principalTable: "Communities",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_UserSubscribed_Users_UserID",
            //             column: x => x.UserID,
            //             principalTable: "Users",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                });

            // migrationBuilder.CreateTable(
            //     name: "Reposts",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //         PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //         CommunityId = table.Column<int>(type: "int", nullable: false),
            //         Vote = table.Column<int>(type: "int", nullable: false),
            //         DateOfCrossPost = table.Column<DateTime>(type: "datetime2", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Reposts", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_Reposts_Communities_CommunityId",
            //             column: x => x.CommunityId,
            //             principalTable: "Communities",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //         table.ForeignKey(
            //             name: "FK_Reposts_Posts_PostId",
            //             column: x => x.PostId,
            //             principalTable: "Posts",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ReplyToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    DateOfComment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RepostId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ReplyToId",
                        column: x => x.ReplyToId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Reposts_RepostId",
                        column: x => x.RepostId,
                        principalTable: "Reposts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReplyToId",
                table: "Comments",
                column: "ReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RepostId",
                table: "Comments",
                column: "RepostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Communities_Name",
            //     table: "Communities",
            //     column: "Name",
            //     unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_PostId",
                table: "Media",
                column: "PostId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Posts_CommunityId",
            //     table: "Posts",
            //     column: "CommunityId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Posts_UserId",
            //     table: "Posts",
            //     column: "UserId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Reposts_CommunityId",
            //     table: "Reposts",
            //     column: "CommunityId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Reposts_PostId",
            //     table: "Reposts",
            //     column: "PostId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_UserModerating_UserID",
            //     table: "UserModerating",
            //     column: "UserID");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Users_Username",
            //     table: "Users",
            //     column: "Username",
            //     unique: true);

            // migrationBuilder.CreateIndex(
            //     name: "IX_UserSubscribed_UserID",
            //     table: "UserSubscribed",
            //     column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "UserModerating");

            migrationBuilder.DropTable(
                name: "UserSubscribed");

            migrationBuilder.DropTable(
                name: "Reposts");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
