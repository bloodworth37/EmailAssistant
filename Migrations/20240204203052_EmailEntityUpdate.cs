using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailAssistant.Migrations
{
    /// <inheritdoc />
    public partial class EmailEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SessionEmailAddress",
                table: "Email",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionEmailAddress",
                table: "Email");
        }
    }
}
