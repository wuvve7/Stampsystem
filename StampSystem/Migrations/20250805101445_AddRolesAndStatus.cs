using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StampSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DirectorRejectionReason",
                table: "SealRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HRRejectionReason",
                table: "SealRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByDirector",
                table: "SealRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByHR",
                table: "SealRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SealRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DirectorRejectionReason",
                table: "SealRequests");

            migrationBuilder.DropColumn(
                name: "HRRejectionReason",
                table: "SealRequests");

            migrationBuilder.DropColumn(
                name: "IsApprovedByDirector",
                table: "SealRequests");

            migrationBuilder.DropColumn(
                name: "IsApprovedByHR",
                table: "SealRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SealRequests");
        }
    }
}
