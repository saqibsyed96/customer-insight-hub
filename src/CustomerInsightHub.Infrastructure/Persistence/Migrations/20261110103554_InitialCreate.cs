using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CustomerInsightHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "City", "Country", "CreatedAtUtc", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Portland", "USA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hello@riverstone-bakery.example", "Riverstone Bakery", "+1-555-0101" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Bristol", "UK", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "contact@blueharbor-logistics.example", "Blue Harbor Logistics", "+44-20-5550-0102" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Toronto", "Canada", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "office@maplegrove-dental.example", "Maple Grove Dental", "+1-555-0103" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
