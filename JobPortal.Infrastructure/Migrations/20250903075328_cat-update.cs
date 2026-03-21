using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class catupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormTypeCategoty",
                table: "FormSections");

            migrationBuilder.AddColumn<bool>(
                name: "IsReadOnly",
                table: "FormTypeCategory",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReadOnly",
                table: "FormTypeCategory");

            migrationBuilder.AddColumn<int>(
                name: "FormTypeCategoty",
                table: "FormSections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
