using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class asfnjk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommInfo",
                table: "Communities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommInfo",
                table: "Communities");
        }
    }
}
