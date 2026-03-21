using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentCityId",
                table: "JobSeekerProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStateId",
                table: "JobSeekerProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesiredCityId",
                table: "JobSeekerProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DesiredStateId",
                table: "JobSeekerProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerProfiles_CurrentCityId",
                table: "JobSeekerProfiles",
                column: "CurrentCityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerProfiles_CurrentStateId",
                table: "JobSeekerProfiles",
                column: "CurrentStateId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerProfiles_DesiredCityId",
                table: "JobSeekerProfiles",
                column: "DesiredCityId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSeekerProfiles_DesiredStateId",
                table: "JobSeekerProfiles",
                column: "DesiredStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerProfiles_Cities_CurrentCityId",
                table: "JobSeekerProfiles",
                column: "CurrentCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerProfiles_Cities_DesiredCityId",
                table: "JobSeekerProfiles",
                column: "DesiredCityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerProfiles_States_CurrentStateId",
                table: "JobSeekerProfiles",
                column: "CurrentStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobSeekerProfiles_States_DesiredStateId",
                table: "JobSeekerProfiles",
                column: "DesiredStateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerProfiles_Cities_CurrentCityId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerProfiles_Cities_DesiredCityId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerProfiles_States_CurrentStateId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_JobSeekerProfiles_States_DesiredStateId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerProfiles_CurrentCityId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerProfiles_CurrentStateId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerProfiles_DesiredCityId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_JobSeekerProfiles_DesiredStateId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropColumn(
                name: "CurrentCityId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropColumn(
                name: "CurrentStateId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropColumn(
                name: "DesiredCityId",
                table: "JobSeekerProfiles");

            migrationBuilder.DropColumn(
                name: "DesiredStateId",
                table: "JobSeekerProfiles");
        }
    }
}
