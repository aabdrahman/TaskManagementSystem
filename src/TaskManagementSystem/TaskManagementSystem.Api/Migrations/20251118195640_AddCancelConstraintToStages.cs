using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCancelConstraintToStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks",
                sql: "[TaskStage] IN ('Development', 'Testing', 'Deployment', 'ChangeManagement', 'Completed', 'Cancelled')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Stage",
                table: "CreatedTasks",
                sql: "[TaskStage] IN ('Development', 'Testing', 'Deployment', 'ChangeManagement', 'Completed')");
        }
    }
}
