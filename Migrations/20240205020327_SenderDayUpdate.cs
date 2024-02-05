using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailAssistant.Migrations
{
    /// <inheritdoc />
    public partial class SenderDayUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Day",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    numEmails = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Day", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionEmailAddress = table.Column<string>(type: "TEXT", nullable: true),
                    SenderAddress = table.Column<string>(type: "TEXT", nullable: true),
                    NumEmails = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sender", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Day");

            migrationBuilder.DropTable(
                name: "Sender");
        }
    }
}
