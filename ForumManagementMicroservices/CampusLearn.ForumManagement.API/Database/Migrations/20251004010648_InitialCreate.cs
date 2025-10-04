using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusLearn.ForumManagement.API.Database.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ForumTopics",
            columns: table => new
            {
                ForumTopicID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ForumTopicTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                ForumTopicDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                RelatedModuleCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                TopicUpVote = table.Column<int>(type: "int", nullable: false),
                ViewCount = table.Column<int>(type: "int", nullable: false),
                TopicCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastActivity = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserProfileID = table.Column<int>(type: "int", nullable: true),
                IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                AnonymousName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsLocked = table.Column<bool>(type: "bit", nullable: false),
                IsPinned = table.Column<bool>(type: "bit", nullable: false),
                IsFeatured = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ForumTopics", x => x.ForumTopicID);
            });

        migrationBuilder.CreateTable(
            name: "ForumTopicResponses",
            columns: table => new
            {
                ResponseID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                MediaContentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ResponseUpVote = table.Column<int>(type: "int", nullable: false),
                ResponseCreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserProfileID = table.Column<int>(type: "int", nullable: true),
                IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                AnonymousName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ForumTopicID = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ForumTopicResponses", x => x.ResponseID);
                table.ForeignKey(
                    name: "FK_ForumTopicResponses_ForumTopics_ForumTopicID",
                    column: x => x.ForumTopicID,
                    principalTable: "ForumTopics",
                    principalColumn: "ForumTopicID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ForumTopicResponses_ForumTopicID",
            table: "ForumTopicResponses",
            column: "ForumTopicID");

        migrationBuilder.CreateIndex(
            name: "IX_ForumTopicResponses_UserProfileID",
            table: "ForumTopicResponses",
            column: "UserProfileID");

        migrationBuilder.CreateIndex(
            name: "IX_ForumTopics_IsPinned",
            table: "ForumTopics",
            column: "IsPinned");

        migrationBuilder.CreateIndex(
            name: "IX_ForumTopics_LastActivity",
            table: "ForumTopics",
            column: "LastActivity");

        migrationBuilder.CreateIndex(
            name: "IX_ForumTopics_RelatedModuleCode",
            table: "ForumTopics",
            column: "RelatedModuleCode");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ForumTopicResponses");

        migrationBuilder.DropTable(
            name: "ForumTopics");
    }
}
