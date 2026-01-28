using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditTrails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParticipantName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParticipandIdentification = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_EntityId",
                table: "AuditTrails",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_EntityName",
                table: "AuditTrails",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_ParticipandIdentification",
                table: "AuditTrails",
                column: "ParticipandIdentification");

            migrationBuilder.CreateIndex(
                name: "IX_AuditTrails_ParticipantName",
                table: "AuditTrails",
                column: "ParticipantName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditTrails");
        }
    }
}
