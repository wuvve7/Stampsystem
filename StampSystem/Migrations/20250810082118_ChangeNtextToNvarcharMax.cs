using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StampSystem.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNtextToNvarcharMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
    name: "FullName",
    table: "AspNetUsers",
    type: "nvarchar(max)",
    nullable: false,
    oldClrType: typeof(string),
    oldType: "ntext");

            migrationBuilder.AlterColumn<string>(
                name: "NationalID",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "ntext");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
    name: "FullName",
    table: "AspNetUsers",
    type: "ntext",
    nullable: false,
    oldClrType: typeof(string),
    oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NationalID",
                table: "AspNetUsers",
                type: "ntext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "ntext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AspNetUsers",
                type: "ntext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RejectionReason",
                table: "AspNetUsers",
                type: "ntext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "ntext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

        }
    }
}
