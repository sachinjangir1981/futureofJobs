using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdduserIdtoJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRemote",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Jobs",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "Positions",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Positions",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Jobs",
                newName: "Location");

            migrationBuilder.AddColumn<bool>(
                name: "IsRemote",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
