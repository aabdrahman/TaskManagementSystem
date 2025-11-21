using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureFilepathandnamepropertymaxlength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreatedTasks_Users_UserId",
                table: "CreatedTasks");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Attachments",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatedTasks_Users_UserId",
                table: "CreatedTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreatedTasks_Users_UserId",
                table: "CreatedTasks");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatedTasks_Users_UserId",
                table: "CreatedTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
