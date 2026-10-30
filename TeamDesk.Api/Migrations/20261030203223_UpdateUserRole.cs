using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamDesk.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELvS9PsS+t505UmPGlCsMXE8r1UEWTE8elW5WLjvmYnBEchMzeIn+Grk3AGM8BLQ0w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELvS9PsS+t505UmPGlCsMXE8r1UEWTE8elW5WLjvmYnBEchMzeIn+Grk3AGM8BLQ0w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELvS9PsS+t505UmPGlCsMXE8r1UEWTE8elW5WLjvmYnBEchMzeIn+Grk3AGM8BLQ0w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "omar@gamil.com", "AQAAAAIAAYagAAAAELvS9PsS+t505UmPGlCsMXE8r1UEWTE8elW5WLjvmYnBEchMzeIn+Grk3AGM8BLQ0w==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Email", "PasswordHash", "Role" },
                values: new object[] { "user@gamil.com", "AQAAAAIAAYagAAAAELvS9PsS+t505UmPGlCsMXE8r1UEWTE8elW5WLjvmYnBEchMzeIn+Grk3AGM8BLQ0w==", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { "omar@gmail.com", "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Email", "PasswordHash", "Role" },
                values: new object[] { "user@gmail.com", "AQAAAAIAAYagAAAAEELvKBcNgw8Mq7htym7+6cCXqpIPc3QyaP4PRTYrdn6CFUnPAV8chPVSXBc5UgCKFQ==", 1 });
        }
    }
}
