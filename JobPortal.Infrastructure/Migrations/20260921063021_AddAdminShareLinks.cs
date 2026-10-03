using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminShareLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTE: `dotnet ef migrations add` also scaffolded a large batch of AddColumn/AlterColumn/
            // RenameColumn/DropColumn statements here for JobSeekerProfiles, FormTypeCategory,
            // FormQuestions, FormOptions, FormAnswers and EmployerProfiles. Those reflect schema drift
            // that already exists on the production database (added outside of EF migrations at some
            // point) rather than anything introduced by this change, so re-applying them would fail or
            // could destructively rename/drop real columns. They were deliberately removed from this
            // migration, which now only adds the new AdminShareLinks table.

            migrationBuilder.CreateTable(
                name: "AdminShareLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormIdsCsv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminShareLinks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminShareLinks_Token",
                table: "AdminShareLinks",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminShareLinks");
        }
    }
}
