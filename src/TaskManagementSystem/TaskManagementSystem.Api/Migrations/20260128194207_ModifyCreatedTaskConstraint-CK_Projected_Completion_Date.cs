using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyCreatedTaskConstraintCK_Projected_Completion_Date : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Projected_Completion_Date",
                table: "CreatedTasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Projected_Completion_Date",
                table: "CreatedTasks",
                sql: "ProjectedCompletionDate > CAST(GETDATE() AS DATE) OR [TaskStage] != 'Cancelled' ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Projected_Completion_Date",
                table: "CreatedTasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Projected_Completion_Date",
                table: "CreatedTasks",
                sql: "ProjectedCompletionDate > CAST(GETDATE() AS DATE)");
        }
    }
}
