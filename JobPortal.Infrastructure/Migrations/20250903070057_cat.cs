using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class cat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormTypeCategoryId",
                table: "FormSections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormTypeCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormCategory = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTypeCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormSections_FormTypeCategoryId",
                table: "FormSections",
                column: "FormTypeCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSections_FormTypeCategory_FormTypeCategoryId",
                table: "FormSections",
                column: "FormTypeCategoryId",
                principalTable: "FormTypeCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSections_FormTypeCategory_FormTypeCategoryId",
                table: "FormSections");

            migrationBuilder.DropTable(
                name: "FormTypeCategory");

            migrationBuilder.DropIndex(
                name: "IX_FormSections_FormTypeCategoryId",
                table: "FormSections");

            migrationBuilder.DropColumn(
                name: "FormTypeCategoryId",
                table: "FormSections");
        }
    }
}
