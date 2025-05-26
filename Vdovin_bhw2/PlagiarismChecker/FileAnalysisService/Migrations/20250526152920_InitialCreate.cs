using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileAnalysisService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisResults",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Paragraphs = table.Column<int>(type: "integer", nullable: false),
                    Words = table.Column<int>(type: "integer", nullable: false),
                    Characters = table.Column<int>(type: "integer", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    IsPlagiarized = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisResults", x => x.FileId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisResults");
        }
    }
}
