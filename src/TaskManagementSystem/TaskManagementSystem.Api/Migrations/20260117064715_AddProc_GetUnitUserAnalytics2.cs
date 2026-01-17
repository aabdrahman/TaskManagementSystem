using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProc_GetUnitUserAnalytics2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    CREATE OR ALTER PROCEDURE [dbo].[Proc_GetUserOrUnitUserTaskAnalytics]
						@StartDate DATE,
						@EndDate DATE,
						@UnitId INT = NULL,
						@UserId INT = NULL
					AS

					BEGIN
		
							SET NOCOUNT ON;

							SELECT [tu].[UserId], [u].[FirstName], [u].[LastName],
								COUNT(*) AS [TotalAssignedTasks], 
								SUM(CASE WHEN ([tu].[CompletionDate] IS NULL AND [tu].[CancelReason] IS NULL) THEN 1 ELSE 0 END) AS [TotalPendingTasks], 
								SUM(CASE WHEN [tu].[CancelReason] IS NOT NULL THEN 1 ELSE 0 END) AS [TotalCancelledTasks],
								SUM(CASE WHEN [tu].[IsDeleted] = 1 THEN 1 ELSE 0 END) AS [TotalDeletedTasks],
								SUM(CASE WHEN [tu].[CompletionDate] IS NULL AND [tu].[CancelReason] IS NULL AND [tu].[ProposedCompletionDate] < CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END) AS [TotalOverdueTasks],
								SUM(CASE WHEN [tu].[CompletionDate] IS NOT NULL THEN 1 ELSE 0 END) AS [TotalCompletedTasks],
								SUM(CASE WHEN [tu].[CompletionDate] IS NOT NULL AND CAST([tu].[CompletionDate] AS date) > [tu].[ProposedCompletionDate] THEN 1 ELSE 0 END) AS [CompletedAfterDueDate],
								SUM(CASE WHEN [tu].[CompletionDate] IS NOT NULL AND CAST([tu].[CompletionDate] AS date) <= [tu].[ProposedCompletionDate] THEN 1 ELSE 0 END) AS [CompletedWithinDueDate],
								CAST(100.0 * (1.0 * SUM(CASE WHEN [tu].[CompletionDate] IS NOT NULL THEN 1 ELSE 0 END) / NULLIF(COUNT(*), 0)) AS decimal(5, 2)) AS [CompletionRate],
								AVG(CASE WHEN [tu].[CompletionDate] IS NOT NULL THEN DATEDIFF(MINUTE, [tu].[ProposedCompletionDate], [tu].[CompletionDate]) END) AS [TotalTimeTaken]

							FROM [dbo].[TaskUsers] [tu]
							JOIN [dbo].[Users] u ON [tu].[UserId] = u.[Id]
							WHERE (CAST([tu].[CreatedAt] AS DATE) BETWEEN @StartDate AND  @EndDate) AND (@UnitId IS NULL OR [u].[UnitId] = @UnitId) AND (@UserId IS NULL OR [tu].UserId = @UserId)
							GROUP BY [tu].UserId, [u].[FirstName], [u].[LastName]
							ORDER BY [CompletionRate] DESC
					END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[Proc_GetUserOrUnitUserTaskAnalytics]");
        }
    }
}
