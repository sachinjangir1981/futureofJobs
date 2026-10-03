using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminSubRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminSubRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AllFormsAccess = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSubRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminSubRoleForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubRoleId = table.Column<int>(type: "int", nullable: false),
                    FormTypeCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSubRoleForms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminSubRoleForms_AdminSubRoles_SubRoleId",
                        column: x => x.SubRoleId,
                        principalTable: "AdminSubRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminSubRolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubRoleId = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminSubRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminSubRolePermissions_AdminSubRoles_SubRoleId",
                        column: x => x.SubRoleId,
                        principalTable: "AdminSubRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminUserSubRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SubRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUserSubRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUserSubRoles_AdminSubRoles_SubRoleId",
                        column: x => x.SubRoleId,
                        principalTable: "AdminSubRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminSubRoleForms_SubRoleId_FormTypeCategoryId",
                table: "AdminSubRoleForms",
                columns: new[] { "SubRoleId", "FormTypeCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminSubRolePermissions_SubRoleId_Section",
                table: "AdminSubRolePermissions",
                columns: new[] { "SubRoleId", "Section" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminSubRoles_Name",
                table: "AdminSubRoles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUserSubRoles_SubRoleId",
                table: "AdminUserSubRoles",
                column: "SubRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUserSubRoles_UserId",
                table: "AdminUserSubRoles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminSubRoleForms");

            migrationBuilder.DropTable(
                name: "AdminSubRolePermissions");

            migrationBuilder.DropTable(
                name: "AdminUserSubRoles");

            migrationBuilder.DropTable(
                name: "AdminSubRoles");
        }
    }
}
