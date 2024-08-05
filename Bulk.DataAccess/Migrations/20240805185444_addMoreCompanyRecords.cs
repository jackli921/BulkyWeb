using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class addMoreCompanyRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "StreetAddress",
                value: "150 Waterloo Ave");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "StreetAddress",
                value: "23 Kitchener Rd");

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                column: "StreetAddress",
                value: "310 Spadina Ave");

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 4, "Innovation Town", "Innovative Tech", "234-567-890", "13131", "CA", "245 Silicon Blvd." },
                    { 5, "Eco City", "Green Energy Inc.", "345-678-901", "14141", "TX", "98 Renewable Rd." },
                    { 6, "Futureville", "Future Enterprises", "456-789-012", "15151", "NY", "77 Futurist Dr." },
                    { 7, "Machine Town", "AI Innovations", "567-890-123", "16161", "WA", "500 AI Ln." },
                    { 8, "Health City", "HealthTech", "678-901-234", "17171", "FL", "300 Wellness Way" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                column: "StreetAddress",
                value: null);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                column: "StreetAddress",
                value: null);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                column: "StreetAddress",
                value: null);
        }
    }
}
