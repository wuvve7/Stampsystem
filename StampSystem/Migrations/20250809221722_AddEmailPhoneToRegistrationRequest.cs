using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StampSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailPhoneToRegistrationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "RegistrationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "RegistrationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "RegistrationRequests");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "RegistrationRequests");
        }
    }
}
