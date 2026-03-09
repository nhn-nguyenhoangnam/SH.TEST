using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BankingApp.Infra.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountHolderName", "AccountNumber", "Balance" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "John Smith", "ACC001", 5000000m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Mary Johnson", "ACC002", 10000000m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Robert Williams", "ACC003", 7500000m },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Patricia Brown", "ACC004", 15000000m },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Michael Davis", "ACC005", 3000000m },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Jennifer Miller", "ACC006", 20000000m },
                    { new Guid("77777777-7777-7777-7777-777777777777"), "William Wilson", "ACC007", 12000000m },
                    { new Guid("88888888-8888-8888-8888-888888888888"), "Linda Moore", "ACC008", 8500000m },
                    { new Guid("99999999-9999-9999-9999-999999999999"), "David Taylor", "ACC009", 6000000m },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Elizabeth Anderson", "ACC010", 25000000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        }
    }
}
