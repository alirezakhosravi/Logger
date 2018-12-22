using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LoggerTest.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "LogRecord",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Logger = table.Column<string>(maxLength: 256, nullable: false),
                    Message = table.Column<string>(nullable: false),
                    Exception = table.Column<string>(nullable: true),
                    StateJson = table.Column<string>(nullable: true),
                    Url = table.Column<string>(maxLength: 1024, nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecord", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Log_Level",
                schema: "dbo",
                table: "LogRecord",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Log_Logger",
                schema: "dbo",
                table: "LogRecord",
                column: "Logger");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogRecord",
                schema: "dbo");
        }
    }
}
