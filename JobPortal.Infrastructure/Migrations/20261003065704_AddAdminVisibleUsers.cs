using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminVisibleUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllUsersAccess",
                table: "AdminUserSubRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AdminUserSubRoleUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignmentId = table.Column<int>(type: "int", nullable: false),
                    TargetUserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUserSubRoleUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminUserSubRoleUsers_AdminUserSubRoles_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "AdminUserSubRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminUserSubRoleUsers_AssignmentId_TargetUserId",
                table: "AdminUserSubRoleUsers",
                columns: new[] { "AssignmentId", "TargetUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUserSubRoleUsers");

            migrationBuilder.DropColumn(
                name: "AllUsersAccess",
                table: "AdminUserSubRoles");
        }
    }
}
