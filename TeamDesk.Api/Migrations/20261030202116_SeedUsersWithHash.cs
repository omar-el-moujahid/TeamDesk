using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TeamDesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersWithHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 4, "omar@gmail.com", "Omar Manager", "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==", 1 },
                    { 5, "user@gmail.com", "user user", "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "TEMP");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "TEMP");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "TEMP");
        }
    }
}
