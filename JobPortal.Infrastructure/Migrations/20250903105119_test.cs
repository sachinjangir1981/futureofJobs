using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Principal;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSections_FormTypeCategory_FormTypeCategoryId",
                table: "FormSections");

            migrationBuilder.AlterColumn<int>(
                name: "FormTypeCategoryId",
                table: "FormSections",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormSections_FormTypeCategory_FormTypeCategoryId",
                table: "FormSections",
                column: "FormTypeCategoryId",
                principalTable: "FormTypeCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormSections_FormTypeCategory_FormTypeCategoryId",
                table: "FormSections");

            migrationBuilder.AlterColumn<int>(
               name: "Id",
               table: "FormSections",
               type: "int",
               nullable: false,
                
               oldClrType: typeof(int),
               oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FormTypeCategoryId",
                table: "FormSections",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FormSections_FormTypeCategory_FormTypeCategoryId",
                table: "FormSections",
                column: "FormTypeCategoryId",
                principalTable: "FormTypeCategory",
                principalColumn: "Id");
        }
    }
}
