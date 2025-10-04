using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusLearn.TopicsManagement.API.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FAQs",
                columns: table => new
                {
                    FAQID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FrequentlyAskedQuestion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    TutorID = table.Column<int>(type: "int", nullable: false),
                    ModuleCode = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQs", x => x.FAQID);
                });

            migrationBuilder.CreateTable(
                name: "QueryTopics",
                columns: table => new
                {
                    QueryTopicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QueryTopicTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    QueryTopicDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedModuleCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TopicCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    IsUrgent = table.Column<bool>(type: "bit", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryTopics", x => x.QueryTopicID);
                });

            migrationBuilder.CreateTable(
                name: "QueryResponses",
                columns: table => new
                {
                    QueryResponseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MediaContentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSolution = table.Column<bool>(type: "bit", nullable: false),
                    HelpfulVotes = table.Column<int>(type: "int", nullable: false),
                    TutorID = table.Column<int>(type: "int", nullable: false),
                    QueryTopicID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryResponses", x => x.QueryResponseID);
                    table.ForeignKey(
                        name: "FK_QueryResponses_QueryTopics_QueryTopicID",
                        column: x => x.QueryTopicID,
                        principalTable: "QueryTopics",
                        principalColumn: "QueryTopicID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FAQs_ModuleCode",
                table: "FAQs",
                column: "ModuleCode");

            migrationBuilder.CreateIndex(
                name: "IX_QueryResponses_QueryTopicID",
                table: "QueryResponses",
                column: "QueryTopicID");

            migrationBuilder.CreateIndex(
                name: "IX_QueryTopics_IsResolved",
                table: "QueryTopics",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_QueryTopics_RelatedModuleCode",
                table: "QueryTopics",
                column: "RelatedModuleCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQs");

            migrationBuilder.DropTable(
                name: "QueryResponses");

            migrationBuilder.DropTable(
                name: "QueryTopics");
        }
    }
}
