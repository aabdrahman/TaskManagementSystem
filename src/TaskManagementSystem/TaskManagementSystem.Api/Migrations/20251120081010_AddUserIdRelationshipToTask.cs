using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdRelationshipToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CreatedTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CreatedTasks_UserId",
                table: "CreatedTasks",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks",
                sql: "[TaskStage] IN ('Development', 'Testing', 'Deployment', 'ChangeManagement', 'Completed', 'Cancelled', 'Review')");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatedTasks_Users_UserId",
                table: "CreatedTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers",
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

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers");

            migrationBuilder.DropIndex(
                name: "IX_CreatedTasks_UserId",
                table: "CreatedTasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CreatedTasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks",
                sql: "[TaskStage] IN ('Development', 'Testing', 'Deployment', 'ChangeManagement', 'Completed', 'Cancelled')");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
