using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagementSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class Reseed_Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9789), "SYSTEM", "Admin" },
                    { 2, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9818), "SYSTEM", "itgovernance" },
                    { 3, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9821), "SYSTEM", "developer" },
                    { 4, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9823), "SYSTEM", "tester" },
                    { 5, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9825), "SYSTEM", "deployment" },
                    { 6, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9829), "SYSTEM", "productowner" },
                    { 7, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9830), "SYSTEM", "businessanalyst" },
                    { 8, new DateTime(2026, 1, 28, 21, 13, 10, 199, DateTimeKind.Local).AddTicks(9832), "SYSTEM", "admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
